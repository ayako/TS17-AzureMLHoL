using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.Devices.Input;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using Newtonsoft.Json;


// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace MLClient
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const double _margin = 2.0;     // セルの配置マージン
        private const double _opacity = 0.2;    // セルの透明度
        private Rectangle _last;                // 最終入力セル

        public MainPage()
        {
            this.InitializeComponent();

            // Gridに行列を作成
            for (int i = 0; i < 8; i++)
                Cells.ColumnDefinitions.Add(new ColumnDefinition());
            for (int j = 0; j < 8; j++)
                Cells.RowDefinitions.Add(new RowDefinition());
            // 正方形で行列を埋める
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var cell = new Rectangle();
                    cell.Fill = new SolidColorBrush(Colors.Blue);
                    cell.Opacity = _opacity;
                    cell.Margin = new Thickness(_margin);
                    cell.SetValue(Grid.RowProperty, row);
                    cell.SetValue(Grid.ColumnProperty, col);
                    cell.PointerPressed += OnCellPressed;
                    cell.PointerEntered += OnCellEntered;
                    Cells.Children.Add(cell);
                }
            }
        }
        private void OnCellPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                var point = e.GetCurrentPoint(null);

                if (point.Properties.IsLeftButtonPressed) //マウス左ボタンのみ
                {
                    var cell = (Rectangle)sender;
                    ToggleCell(cell);  //セルのトグル(On&Off 切り替え)
                    _last = cell;
                }
            }
        }

        private void OnCellEntered(object sender, PointerRoutedEventArgs e)
        {
            var cell = (Rectangle)sender;

            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                var point = e.GetCurrentPoint(null);

                if (!point.Properties.IsLeftButtonPressed)
                    return;  //マウス左ボタン以外は無視

                if (cell == _last)
                {
                    _last = null;
                    return; // 入力中は無視
                }
            }

            ToggleCell(cell);
        }

        private void ToggleCell(Rectangle cell)
        {
            // セルのトグル(もう一度クリックすると On <-> Off 切り替え)
            cell.Opacity = (cell.Opacity < 1.0) ? 1.0 : _opacity;
        }

        private async void OnSubmit(object sender, RoutedEventArgs e)
        {
            // 画面の入力を取得
            string[] values = new string[65];

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    int index = (row * 8) + col;
                    values[index] =
                        ((Rectangle)Cells.Children[index]).Opacity == 1.0 ? "16" : "0";
                }
            }

            values[64] = "0"; // digit 値は0に固定

            try
            {
                // ML呼び出し
                await MLSubmitAsync(values);
            }
            catch (Exception ex)
            {
                // エラーハンドラー (エラーメッセージを画面表示)
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }
        }

        private void OnClear(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 64; i++)
                ((Rectangle)Cells.Children[i]).Opacity = _opacity;
        }

        private async Task MLSubmitAsync(string[] v)
        {
            using (var client = new HttpClient())
            {
                // ML に送信するデータセットの作成
                var scoreRequest = new
                {
                    Inputs = new Dictionary<string, List<Dictionary<string, string>>>()
                    {
                        {
                            "input1",
                            new List<Dictionary<string, string>>()
                            {
                                new Dictionary<string, string>()
                                {
                                    { "p01" , v[0] }, { "p02" , v[1] }, { "p03" , v[2] }, { "p04" , v[3] },
                                    { "p05" , v[4] }, { "p06" , v[5] }, { "p07" , v[6] }, { "p08" , v[7] },
                                    { "p09" , v[8] }, { "p10" , v[9] }, { "p11" , v[10] }, { "p12" , v[11] },
                                    { "p13" , v[12] }, { "p14" , v[13] }, { "p15" , v[14] }, { "p16" , v[15] },
                                    { "p17" , v[16] }, { "p18" , v[17] }, { "p19" , v[18] }, { "p20" , v[19] },
                                    { "p21" , v[20] }, { "p22" , v[21] }, { "p23" , v[22] }, { "p24" , v[23] },
                                    { "p25" , v[24] }, { "p26" , v[25] }, { "p27" , v[26] }, { "p28" , v[27] },
                                    { "p29" , v[28] }, { "p30" , v[29] }, { "p31" , v[30] }, { "p32" , v[31] },
                                    { "p33" , v[32] }, { "p34" , v[33] }, { "p35" , v[34] }, { "p36" , v[35] },
                                    { "p37" , v[36] }, { "p38" , v[37] }, { "p39" , v[38] }, { "p40" , v[39] },
                                    { "p41" , v[40] }, { "p42" , v[41] }, { "p43" , v[42] }, { "p44" , v[43] },
                                    { "p45" , v[44] }, { "p46" , v[45] }, { "p47" , v[46] }, { "p48" , v[47] },
                                    { "p49" , v[48] }, { "p50" , v[49] }, { "p51" , v[50] }, { "p52" , v[51] },
                                    { "p53" , v[52] }, { "p54" , v[53] }, { "p55" , v[54] }, { "p56" , v[55] },
                                    { "p57" , v[56] }, { "p58" , v[57] }, { "p59" , v[58] }, { "p60" , v[59] },
                                    { "p61" , v[60] }, { "p62" , v[61] }, { "p63" , v[62] }, { "p64" , v[63] },
                                    { "digit", v[64] },
                                }
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };

                //「api_key」にご自分の Api キーをコピーしてください
                const string apiKey = "api_key";
                client.DefaultRequestHeaders.Authorization
                    = new AuthenticationHeaderValue("Bearer", apiKey);
                //「web_service_url」にご自分の サービスURLをコピーしてください
                client.BaseAddress = new Uri("web_service_url");

                // ML にアクセスして結果を取得
                HttpResponseMessage response
                    = await client.PostAsJsonAsync("", scoreRequest).ConfigureAwait(false);

                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        dynamic result
                            = JsonConvert.DeserializeObject<dynamic>(json);
                        var digit = result.Results.output1[0]["Scored Labels"];
                        var dialog = new MessageDialog
                            (String.Format("数字の {0} かな？", digit));
                        await dialog.ShowAsync();
                    }
                    else
                    {
                        var dialog = new MessageDialog
                        (String.Format("MLの呼び出しに失敗しました。\n"
                            + "status code: { 0 }", response.StatusCode));
                        await dialog.ShowAsync();
                    }
                }
                );
            }
        }
        

    }
}
