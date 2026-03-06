// =============================================================
// ProPla Outlook Mail 
// =============================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;
using Word = Microsoft.Office.Interop.Word;

namespace ProPla.Mail
{
    /// <summary>
    /// メール作成に使用する設定情報を保持するクラス。
    /// 宛先・本文テンプレートのファイルパス、件名のプレフィックス、
    /// HTMLインラインCSS、Wordエディタ取得リトライ条件などを指定します。
    /// </summary>
    public sealed class MailComposerOptions
    {
        /// <summary>宛先アドレスを列挙したテキストファイルのパス。</summary>
        public string MailToPath { get; set; } = @"Z:\\商品企画室\\OutlookMail\\MailTo.txt";

        /// <summary>本文テンプレート（HTML）のファイルパス。</summary>
        public string BodyTemplatePath { get; set; } = @"Z:\\商品企画室\\OutlookMail\\Mail.html";

        /// <summary>件名の先頭に付与する固定文字列。</summary>
        public string SubjectPrefix { get; set; } = "【商品情報の更新】";

        /// <summary>本文HTMLへ挿入するインラインCSS。</summary>
        public string CssInline { get; set; } =
            "p{margin-top:0;margin-bottom:0;} .ins{background:#c6f6d5;} .del{background:#fed7d7;text-decoration:line-through;} .field{margin:8px 0;} .field__title{font-weight:bold;}";

        /// <summary>Wordエディタ取得時の最大リトライ回数。</summary>
        public int EditorProbeMaxTries { get; set; } = 50;

        /// <summary>Wordエディタ取得時のリトライ間隔（ミリ秒）。</summary>
        public int EditorProbeDelayMs { get; set; } = 100;
    }

    /// <summary>
    /// メール作成処理で発生した例外を表します。
    /// </summary>
    public sealed class MailCompositionException : Exception
    {
        /// <summary>
        /// メッセージを指定して <see cref="MailCompositionException"/> を生成します。
        /// </summary>
        /// <param name="message">エラーメッセージ。</param>
        public MailCompositionException(string message) : base(message) { }

        /// <summary>
        /// メッセージと内部例外を指定して <see cref="MailCompositionException"/> を生成します。
        /// </summary>
        /// <param name="message">エラーメッセージ。</param>
        /// <param name="inner">内部例外。</param>
        public MailCompositionException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// メールテンプレートの読み込み・組み立て処理を提供するインターフェースです。
    /// </summary>
    public interface ITemplateProvider
    {
        /// <summary>宛先文字列を非同期に読み込みます。</summary>
        /// <param name="ct">キャンセルトークン。</param>
        /// <returns>宛先（カンマ区切り等）。</returns>
        Task<string> ReadToAsync(CancellationToken ct = default);

        /// <summary>本文テンプレート（HTML）を非同期に読み込みます。</summary>
        /// <param name="ct">キャンセルトークン。</param>
        /// <returns>HTML文字列。</returns>
        Task<string> ReadBodyTemplateAsync(CancellationToken ct = default);

        /// <summary>指定のHTML文字列の &lt;head&gt; に CSS を挿入します。</summary>
        /// <param name="html">対象HTML。</param>
        /// <param name="css">挿入するCSS。</param>
        /// <returns>CSS挿入後のHTML。</returns>
        string InsertStyleIntoHead(string html, string css);
    }

    /// <summary>
    /// ファイルからテンプレート（宛先・本文HTML）を読み込む実装です。
    /// </summary>
    public sealed class FileTemplateProvider : ITemplateProvider
    {
        private readonly MailComposerOptions _options;

        /// <summary>
        /// オプションを指定してインスタンスを生成します。
        /// </summary>
        /// <param name="options">メール作成オプション。</param>
        public FileTemplateProvider(MailComposerOptions options) => _options = options;

        /// <inheritdoc />
        public async Task<string> ReadToAsync(CancellationToken ct = default)
        {
            var path = _options.MailToPath;
            if (!File.Exists(path)) throw new MailCompositionException($"宛先ファイルが見つかりません: {path}");
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sr = new StreamReader(fs, Encoding.UTF8))
            {
                return await sr.ReadToEndAsync().ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public async Task<string> ReadBodyTemplateAsync(CancellationToken ct = default)
        {
            var path = _options.BodyTemplatePath;
            if (!File.Exists(path)) throw new MailCompositionException($"本文テンプレートが見つかりません: {path}");
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sr = new StreamReader(fs, Encoding.UTF8))
            {
                var html = await sr.ReadToEndAsync().ConfigureAwait(false);
                // 改行削除（OutlookのHTML扱い簡素化のため）
                return html.Replace("\r", string.Empty).Replace("\n", string.Empty);
            }
        }

        /// <inheritdoc />
        public string InsertStyleIntoHead(string html, string css)
        {
            if (string.IsNullOrWhiteSpace(html)) return html;

            var style = $"<style>{css}</style>";
            var lower = html.ToLowerInvariant();
            var headIdx = lower.IndexOf("<head>", StringComparison.Ordinal);
            if (headIdx >= 0)
            {
                var insertPos = headIdx + "<head>".Length;
                return html.Insert(insertPos, style);
            }

            // <head>が無い場合は包む
            var bodyIdx = lower.IndexOf("<body", StringComparison.Ordinal);
            if (bodyIdx >= 0)
            {
                return $"<html><head>{style}</head>{html}</html>";
            }

            return $"<html><head>{style}</head><body>{html}</body></html>";
        }
    }

    /// <summary>
    /// 差分HTMLを生成するサービスのインターフェースです。
    /// </summary>
    public interface IHtmlDiffService
    {
        /// <summary>
        /// 2つのテキストの差分を &lt;span class="ins"&gt; と &lt;span class="del"&gt; で表現したHTMLを生成します。
        /// </summary>
        /// <param name="beforeText">変更前テキスト。</param>
        /// <param name="afterText">変更後テキスト。</param>
        /// <returns>差分表現済みHTML。</returns>
        string BuildDiffHtml(string beforeText, string afterText);
    }

    /// <summary>
    /// 文字単位（拡張書記素単位）で簡易的に差分を計算し、
    /// 追加（ins）・削除（del）を強調するHTMLを生成する実装です。
    /// </summary>
    public sealed class HtmlDiffService : IHtmlDiffService
    {
        private const int Lookahead = 40;

        public string BuildDiffHtml(string beforeText, string afterText)
        {
            var before = Graphemes(beforeText ?? string.Empty);
            var after = Graphemes(afterText ?? string.Empty);

            var sb = new StringBuilder();
            int i = 0, j = 0;

            while (i < before.Count && j < after.Count)
            {
                // 1. 文字が一致する場合（変更なし）
                if (before[i] == after[j])
                {
                    sb.Append(WebUtility.HtmlEncode(before[i]));
                    i++; j++;
                    continue;
                }

                // 2. 不一致の場合、前方を探す
                var aheadI = IndexOf(after, before[i], j, Lookahead);
                var aheadJ = IndexOf(before, after[j], i, Lookahead);

                if (aheadI >= 0)
                {
                    // after の方に現在の文字が見つかった → その間は「追加(ins)」された
                    sb.Append("<span class=\"ins\">");
                    while (j < aheadI) sb.Append(WebUtility.HtmlEncode(after[j++]));
                    sb.Append("</span>");
                    // 同期した位置から再開（この時 i と j は一致するはず）
                }
                else if (aheadJ >= 0)
                {
                    // before の方に現在の文字が見つかった → その間は「削除(del)」された
                    sb.Append("<span class=\"del\">");
                    while (i < aheadJ) sb.Append(WebUtility.HtmlEncode(before[i++]));
                    sb.Append("</span>");
                }
                else
                {
                    // どちらも見つからない → 置換（削除と追加の連続）
                    sb.Append("<span class=\"del\">").Append(WebUtility.HtmlEncode(before[i++])).Append("</span>");
                    sb.Append("<span class=\"ins\">").Append(WebUtility.HtmlEncode(after[j++])).Append("</span>");
                }
            }

            // 残った before を削除として処理
            if (i < before.Count)
            {
                sb.Append("<span class=\"del\">");
                while (i < before.Count) sb.Append(WebUtility.HtmlEncode(before[i++]));
                sb.Append("</span>");
            }

            // 残った after を追加として処理
            if (j < after.Count)
            {
                sb.Append("<span class=\"ins\">");
                while (j < after.Count) sb.Append(WebUtility.HtmlEncode(after[j++]));
                sb.Append("</span>");
            }

            return sb.ToString();
        }

        private static int IndexOf(List<string> list, string target, int start, int maxLook)
        {
            var end = Math.Min(list.Count, start + maxLook);
            for (int k = start; k < end; k++)
                if (list[k] == target) return k;
            return -1;
        }

        private static List<string> Graphemes(string s)
        {
            var enumerator = StringInfo.GetTextElementEnumerator(s);
            var list = new List<string>();
            while (enumerator.MoveNext()) list.Add((string)enumerator.Current);
            return list;
        }
    }


    /// <summary>
    /// Outlook を介してメールを作成・表示するためのインターフェースです。
    /// </summary>
    public interface IOutlookMailClient : IDisposable
    {
        /// <summary>新しい <see cref="Outlook.MailItem"/> を作成します。</summary>
        /// <returns>作成された MailItem。</returns>
        Outlook.MailItem CreateMail();

        /// <summary>
        /// メールの <see cref="Outlook.Inspector"/> から Word エディタを取得します。
        /// 取得できるまで指定回数リトライします。
        /// </summary>
        /// <param name="mail">対象メール。</param>
        /// <param name="maxTries">最大試行回数。</param>
        /// <param name="delayMs">リトライ間隔（ミリ秒）。</param>
        /// <param name="ct">キャンセルトークン。</param>
        /// <returns>取得できた Word.Document。取得失敗時は null。</returns>
        Task<Word.Document> TryGetWordEditorAsync(Outlook.MailItem mail, int maxTries, int delayMs, CancellationToken ct);
    }

    /// <summary>
    /// Outlook の COM API を用いて MailItem の作成および Word エディタ取得を行う実装です。
    /// </summary>
    public sealed class OutlookMailClient : IOutlookMailClient
    {
        private Outlook.Application _app;
        private bool _disposed;

        /// <inheritdoc />
        public Outlook.MailItem CreateMail()
        {
            try
            {
                if (_app == null) _app = new Outlook.Application();
                var mail = _app.CreateItem(Outlook.OlItemType.olMailItem) as Outlook.MailItem;
                if (mail == null) throw new MailCompositionException("OutlookのMailItem作成に失敗しました。");
                return mail;
            }
            catch (COMException com)
            {
                throw new MailCompositionException("Outlookの起動/作成に失敗しました。", com);
            }
        }

        /// <inheritdoc />
        public async Task<Word.Document> TryGetWordEditorAsync(Outlook.MailItem mail, int maxTries, int delayMs, CancellationToken ct)
        {
            Outlook.Inspector inspector = null;
            try
            {
                inspector = mail.GetInspector;
                for (int i = 0; i < maxTries; i++)
                {
                    ct.ThrowIfCancellationRequested();
                    try
                    {
                        if (inspector.EditorType == Outlook.OlEditorType.olEditorWord)
                        {
                            var doc = inspector.WordEditor as Word.Document;
                            if (doc != null) return doc;
                        }
                    }
                    catch { /* Inspectorの状態遷移中は例外になり得るため握りつぶす */ }
                    await Task.Delay(delayMs, ct).ConfigureAwait(false);
                }
                return null;
            }
            finally
            {
                if (inspector != null)
                {
                    try { Marshal.FinalReleaseComObject(inspector); } catch { }
                }
            }
        }

        /// <summary>
        /// COMオブジェクトを解放します。
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (_app != null)
            {
                try { Marshal.FinalReleaseComObject(_app); } catch { }
                _app = null;
            }
        }
    }

    /// <summary>
    /// 商品情報の変更をHTML差分として本文に埋め込み、Outlook の作成ウィンドウを表示するための中核クラスです。
    /// テンプレート読み込み、件名・宛先設定、差分生成、Wordエディタの段落間余白調整までを行います。
    /// </summary>
    public sealed class ProPlaOutlookMailer : IDisposable
    {
        private readonly MailComposerOptions _options;
        private readonly ITemplateProvider _templates;
        private readonly IHtmlDiffService _diff;
        private readonly IOutlookMailClient _outlook;

        private string _mailBody = string.Empty;
        private string _changesHtml = string.Empty;
        private Outlook.MailItem _mailItem;
        private bool _disposed;

        /// <summary>
        /// 依存関係（テンプレートプロバイダ、差分サービス、Outlookクライアント）を受け取り、
        /// メーラーを初期化します。いずれも null の場合はデフォルト実装が使用されます。
        /// </summary>
        /// <param name="options">メール作成オプション。</param>
        /// <param name="templateProvider">テンプレート読み込みの提供元。null 可。</param>
        /// <param name="diffService">差分生成サービス。null 可。</param>
        /// <param name="outlookClient">Outlook クライアント。null 可。</param>
        public ProPlaOutlookMailer(MailComposerOptions options,
            ITemplateProvider templateProvider = null,
            IHtmlDiffService diffService = null,
            IOutlookMailClient outlookClient = null)
        {
            _options = options;
            _templates = templateProvider ?? new FileTemplateProvider(options);
            _diff = diffService ?? new HtmlDiffService();
            _outlook = outlookClient ?? new OutlookMailClient();
        }

        /// <summary>
        /// メールを初期化し、件名・宛先・本文テンプレートを設定します。
        /// </summary>
        /// <param name="productNameOfficial">正式商品名。件名および本文のプレースホルダーに使用。</param>
        /// <param name="ct">キャンセルトークン。</param>
        /// <exception cref="MailCompositionException">テンプレートの読込やMailItem作成に失敗した場合。</exception>
        public async Task InitializeAsync(string productNameOfficial, CancellationToken ct = default)
        {
            var to = await _templates.ReadToAsync(ct).ConfigureAwait(false);
            var body = await _templates.ReadBodyTemplateAsync(ct).ConfigureAwait(false);

            _mailItem = _outlook.CreateMail();
            _mailItem.Subject = _options.SubjectPrefix + productNameOfficial;
            _mailItem.To = to;
            _mailBody = body.Replace("%商品名%", productNameOfficial);
        }

        /// <summary>
        /// 1フィールド分の変更（前後値）を差分HTMLに変換し、本文の「詳細」セクションへ追加します。
        /// </summary>
        /// <param name="fieldDisplayName">フィールドの表示名。</param>
        /// <param name="beforeText">変更前の値。</param>
        /// <param name="afterText">変更後の値。</param>
        public void AppendChangeSection(string fieldDisplayName, string beforeText, string afterText)
        {
            if (string.IsNullOrWhiteSpace(fieldDisplayName)) return;
            var title = WebUtility.HtmlEncode(fieldDisplayName);
            //var diff = _diff.BuildDiffHtml(beforeText ?? string.Empty, afterText ?? string.Empty);
            //_changesHtml += $"<div class=\"field\"><p class=\"field__title\">■{title}</p>{diff}</div>";

            string _s2 = "";
            _s2 += "<font face=\"BIZ UDPゴシック\" size=\"3\"  color=\"black\">";
            _s2 += "  <p>■%項目名%</p>";
            _s2 += "</font>";

            _s2 += "<font face=\"BIZ UDPゴシック\" size=\"3\"  color=\"black\">";
            _s2 += "  <p><span style=\"background-color: silver;\">≪更新前≫</span></p>";
            _s2 += "  <p>　%更新前%</p>";
            _s2 += "  <p><span style=\"background-color: deepskyblue;\">≪更新後≫</span></p>";
            _s2 += "</font>";

            _s2 += "<font face=\"BIZ UDPゴシック\" size=\"3\"  color=\"red\">";
            _s2 += "  <p>　%更新後%</p>";
            _s2 += "  <p> </p>";
            _s2 += "</font>";
            _s2 = _s2.Replace("%項目名%", fieldDisplayName);

            _s2 = _s2.Replace("%更新前%", beforeText);

            _s2 = _s2.Replace("%更新後%", afterText);

            _changesHtml += _s2;

        }

        /// <summary>
        /// Outlook のメール作成ウィンドウを表示し、HTML 本文を設定します。
        /// Word エディタを取得できた場合は段落の前後余白をゼロに調整します。
        /// </summary>
        /// <param name="ct">キャンセルトークン。</param>
        /// <exception cref="MailCompositionException">初期化前に呼び出された場合。</exception>
        public async Task ShowInspectorAsync(CancellationToken ct = default)
        {
            if (_mailItem == null) throw new MailCompositionException("メールが初期化されていません。");

            var html = _mailBody.Replace("%詳細%", _changesHtml);
            html = _templates.InsertStyleIntoHead(html, _options.CssInline);

            _mailItem.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;
            _mailItem.HTMLBody = html;
            _mailItem.Display(false);

            Word.Document doc = null;
            try
            {
                doc = await _outlook.TryGetWordEditorAsync(_mailItem, _options.EditorProbeMaxTries, _options.EditorProbeDelayMs, ct).ConfigureAwait(false);
                if (doc != null)
                {
                    // Wordの段落設定を調整（余白をゼロに）
                    Word.Range rng = null;
                    Word.ParagraphFormat fmt = null;
                    try
                    {
                        rng = doc.Content;
                        fmt = rng.ParagraphFormat;
                        fmt.SpaceBeforeAuto = 0;
                        fmt.SpaceAfterAuto = 0;
                        fmt.SpaceBefore = 0f;
                        fmt.SpaceAfter = 0f;
                    }
                    finally
                    {
                        if (fmt != null) { try { Marshal.FinalReleaseComObject(fmt); } catch { } }
                        if (rng != null) { try { Marshal.FinalReleaseComObject(rng); } catch { } }
                    }
                }
            }
            finally
            {
                if (doc != null) { try { Marshal.FinalReleaseComObject(doc); } catch { } }
            }
            _mailItem.Display(true);
        }

        /// <summary>
        /// Outlook の MailItem および保持する COM オブジェクトを解放します。
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (_mailItem != null)
            {
                try { Marshal.FinalReleaseComObject(_mailItem); } catch { }
                _mailItem = null;
            }
            _outlook.Dispose();
        }
    }
}
