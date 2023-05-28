using System;
using static System.Console;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Xml;
using System.Threading;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.Diagnostics;

using System.Net.Http;
using HtmlAgilityPack;

using Xamarin.Essentials;
using Newtonsoft.Json;


namespace TizenXamlApp1
{
    public partial class MainPage : ContentPage
    {
        // 노래 제목을 저장하는 리스트
        public List<string> song = new List<string>();
        
        // 가수 이름을 저장하는 리스트
        public List<string> singer = new List<string>();

        // 뉴스 헤더라인을 저장하는 리스트 
        public List<string> news = new List<string>();

        // 토끼 사진의 좌우 반전을 위한 변수
        private bool isFlipped;

        // 1초를 나타내는 상수 
        public const int second = 1000;

        public MainPage()
        {
            InitializeComponent();
            ChangeBackground();
            Digital_Clock();
            Analog_Clock();
            GetDate();
            Rec_song();
            Display_weather_news_info();
            StartSnowfall();
            StartStarfall();            

            // 1초마다 이미지를 좌우로 반전시키는 타이머 설정
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                FlipImage();
                return true; // 반복적으로 호출하려면 true를 반환합니다.
            });
        }

        // 눈이 내리는 애니메이션을 3ms마다 반복적으로 재생
        private async void StartSnowfall()
        {
            while (true)
            {
                await CreateSnowflake();
                await Task.Delay(3);
            }
        }

        // 전체 화면에서 눈이 내리는 애니메이션 기능
        private async Task CreateSnowflake()
        {

            // 눈 사진의 초기 위치를 설정합니다.
            AbsoluteLayout.SetLayoutBounds(snow_image, new Rectangle(0, 0, 1920, 1080));
            AbsoluteLayout.SetLayoutFlags(snow_image, AbsoluteLayoutFlags.PositionProportional);

            // 눈이 아래로 내려오는 애니메이션을 생성하며 15000ms의 시간이 지나면 사진이 화면 완전히 아래로 사라집니다.
            await snow_image.TranslateTo(snow_image.X, snow_image.Height, 15000); // 눈이 아래로 이동합니다.
            snow_image.TranslationY = 0; // 눈의 위치를 초기화합니다.
        }

        // 별똥별이 떨어지는 애니메이션을 3ms마다 반복적으로 재생
        private async void StartStarfall()
        {
            while (true)
            {
                await CreateStarAnimation();
                await Task.Delay(3);
            }
        }

        // 별똥별이 대각선으로 자연스럽게 떨어지는 애니메이션 기능 
        private async Task CreateStarAnimation()
        {

            double startX = 0; // 시작 위치 X 좌표
            double startY = 0; // 시작 위치 Y 좌표
            double endX = 1920; // 목표 위치 X 좌표
            double endY = 1080; // 목표 위치 Y 좌표
            uint duration = 10000; // 애니메이션 지속 시간 (밀리초)

            // 시작 위치에서 목표 위치까지 이동
            await star_image.TranslateTo(endX, endY, duration);

            // 별똥별 애니메이션의 자연스러움을 위해 X좌표가 100씩 증가할 때마다 0.03씩 Opacity 값을 감소
            double opacity = 1.0;
            double stepSize = 0.03;
            double currentX = startX;
            while (currentX < endX)
            {
                opacity -= stepSize;
                star_image.Opacity = opacity;

                // Check if opacity has reached 0 or below
                if (opacity <= 0)
                {
                    // Reset the opacity to 1.0
                    star_image.Opacity = 1.0;
                    break;
                }

                currentX += 100;
                await star_image.TranslateTo(currentX, endY, 0); // Move to the next X coordinate
                await Task.Delay(100);
            }

            // 애니메이션 종료 후 초기 위치로 이동
            star_image.TranslationX = startX;
            star_image.TranslationY = startY;
        }

        // 배경화면 변경 기능 (2장의 사진을 10초 간격으로 Fade in out으로 변경)
        private async void ChangeBackground()
        {
            int background_idx = 0;
            string[] background_arr = { "winter1.jpg", "background2.jpg" };
            while (true)
            {
                await background_image.FadeTo(0, second);
                background_idx++; background_idx %= 2;
                background_image.Source = background_arr[background_idx];
                await background_image.FadeTo(1, second);
                await Task.Delay(10 * second);
            }
        }

        // 디지털 시계 기능
        private async void Digital_Clock()
        {

            while (true)
            {
                DateTime time = DateTime.Now;
                digital_clock.Text = time.ToString("HH:mm:ss");

                await Task.Delay(100);
            }
        }

        // 아날로그 시계 기능
        private async void Analog_Clock()
        {
            // 1초에 360/60도 = 6도
            // 1분에 360/60도 = 6도
            // 1시간에 360/12 = 30도

            while (true)
            {
                hour_hand.RotateTo(DateTime.Now.Hour * 30, 10);
                min_hand.RotateTo(DateTime.Now.Minute * 6, 10);
                sec_hand.RotateTo(DateTime.Now.Second * 6, 10);
                await Task.Delay(100);
            }
        }

        // 날짜 기능
        private async void GetDate()
        {
            DateTime data = DateTime.Today;
            date_label.Text = data.ToString("yyyy-MM-dd dddd");
        }


        // 토끼의 애니메이션 효과 기능 
        private void FlipImage()
        {
            if (isFlipped)
            {
                rabbit.ScaleX = 1; // 원래 크기로 설정하여 이미지를 원래대로 돌립니다.
            }
            else
            {
                rabbit.ScaleX = -1; // 이미지를 좌우로 반전시킵니다.
            }

            isFlipped = !isFlipped; // 상태를 반전시킵니다.
        }

        // 웹 크롤링을 통한 멜론 실시간 인기차트 노래의 제목과 가수 이름 저장하는 기능 
        private async Task StartCrowlerAsync()
        {
            // url 변수에 크롤링할 웹 페이지(멜론 사이트) 저장
            string url = "https://www.melon.com/index.htm";

            // 웹페이지의 HTML 소스를 가져오기
            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8; // Set the encoding explicitly to UTF-8 
            string html = await webClient.DownloadStringTaskAsync(url);

            // 가져온 HTML 소스를 htmlDocument에 로드하기
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            for (int i = 1; i <= 2; i++)
            {
                for (int j = 1; j <= 5; j++)
                {
                    // 노래 제목과 가수의 이름의 XPath를 발췌하여 HTML 문서에서 해당 노드를 선택하기
                    HtmlNode node = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='conts']/div[5]/div/ul/li[" + i.ToString() + "]/div/ul/li[" + j.ToString() + "]/div[2]/div[2]/p");
                    HtmlNode node2 = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='conts']/div[5]/div/ul/li[" + i.ToString() + "]/div/ul/li[" + j.ToString() + "]/div[2]/div[2]/div/div");

                    if (node != null)
                    {
                        // 노드에서 내부의 a태그 선택하기 
                        HtmlNode aTag = node.SelectSingleNode(".//a");
                        HtmlNode aTag2 = node2.SelectSingleNode(".//a");

                        // a 태그의 내용을 가져와서 전처리 작업 수행
                        string temp_text1 = aTag.InnerText.Trim();
                        string temp_text2 = aTag2.InnerText.Trim();

                        // 특정 HTML 엔터티 '로 대체하기 
                        string text1 = temp_text1.Replace("&#39;", "'");
                        string text2 = temp_text2.Replace("&#39;", "'");

                        // 전처리 작업이 완료 되었으면 노래제목과 가수이름의 리스트에 추가
                        song.Add(text1);
                        singer.Add(text2);
                    }
                }
            }
        }

        // 화면에 노래 제목과 가수 이름 출력하기 
        private async void Rec_song()
        {
            // 가수와 곡 정보를 크롤링하기
            await StartCrowlerAsync();

            // Random 클래스를 이용하여 랜덤한 인덱스를 선택
            Random random = new Random();

            // 랜덤 노래의 제목과 가수 이름 화면에 표시
            int index = random.Next(0, singer.Count);
            singer_song_name.Text = singer[index] + "  -  " + song[index];
            
            // 화면의 상단 바에 왼쪽에서 오른쪽으로
            // 슬라이딩 애니메이션 효과로 노래 제목과 가수 이름 표시
            while (true)
            {
                await singer_song_name.TranslateTo(150, 0, 0);
                await singer_song_name.TranslateTo(1920 - 250, 0, 13 * second);
            }
        }

        // 한국 주요 도시의 현재 날씨 불러오기 
        private async Task<List<string>> GetWeatherByLocationAsync()
        {
            // OpenWeatherMap API를 사용하기 위한 API 키 및 도시들의 정보 정의
            string apiKey = "c62e681083cbe7b38584499ffbdc3437";
            string[] cities = { "Seoul", "Incheon", "Daegu", "Busan", "Daejeon", "Gwangju", "Cheongju" };

            List<string> weatherList = new List<string>();

            try
            {
                // HTTP 요청을 보내기 위한 client 인스턴스 생성 
                using (HttpClient client = new HttpClient())
                {
                    // 각 도시들에 대한 날씨 정보 조회
                    foreach (string city in cities)
                    {
                        // 날씨 정보를 조회하기 위한 URL 저장
                        string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}";

                        // API에 GET요청을 보내고 응답을 response 변수에 저장 
                        HttpResponseMessage response = await client.GetAsync(apiUrl);
                        
                        // 응답 성공 시
                        if (response.IsSuccessStatusCode)
                        {
                            // 응답 본문을 json에 문자열로 저장
                            string json = await response.Content.ReadAsStringAsync();

                            // json 데이터에서 온도와 날씨 설명을 추출
                            dynamic data = JsonConvert.DeserializeObject(json);
                            double temperatureKelvin = data["main"]["temp"];
                            double temperatureCelsius = temperatureKelvin - 273.15; // 켈빈(Kelvin)을 섭씨(Celsius)로 변환
                            string weatherDescription = data["weather"][0]["description"];

                            // 온도와 날씨 정보를 문자열로 구성하여 weatherList에 추가 
                            string weatherInfo = $"{city}: {temperatureCelsius:F2}°C, {weatherDescription}";
                            weatherList.Add(weatherInfo);
                        }
                        else
                        {
                            // 응답 실패 시 에러 메시지를 리스트에 추가 
                            string errorMessage = $"Failed to retrieve weather information for {city}: {response.StatusCode}";
                            weatherList.Add(errorMessage);
                        }
                    }
                }
            }
            // 예외가 발생한 경우 에러 메시지를 리스트에 추가 
            catch (Exception ex)
            {
                string errorMessage = $"Exception occurred: {ex.Message}";
                weatherList.Add(errorMessage);
            }

            return weatherList;
        }

        
        private async Task<List<string>> FetchNewsAsync()
        {
            // 뉴스의 헤드라인을 저장하기 위한 리스트 생성
            List<string> news_List = new List<string>();

            // HTTP 요청을 위한 client 인스턴스 생성 
            HttpClient client = new HttpClient();

            // GetAsync 메서드를 사용하여 지정된 URL(연합뉴스)에서 GET 요청 보내기
            HttpResponseMessage response = await client.GetAsync("https://www.yonhapnewstv.co.kr/browse/feed/");

            // 응답으로부터 데이터를 문자열로 가져오기
            string responseData = await response.Content.ReadAsStringAsync();

            // 데이터를 XML형식으로 로드 
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(responseData);

            // SelectNode 메서드를 사용하여 XML 문서에서 item 요소 선택
            XmlNodeList ItemNodes = xmlDoc.SelectNodes("//item");

            int idx = 0;

            // 반복문을 통해 각 item 요소에서 title 요소를 찾아 뉴스 제목 추출 
            foreach (XmlNode itemNode in ItemNodes)
            {
                var context = itemNode.SelectSingleNode("title");
                string news = context.InnerText;
                news_List.Add(news);
                idx++;
                if (idx == 7) break;
            }
            return news_List;
        }

        // 날씨와 뉴스 정보를 화면에 보여주는 기능 
        private async void Display_weather_news_info()
        {
            string[,] weatherArray = new string[,]
            {
        { "Seoul", string.Empty },
        { "Incheon", string.Empty },
        { "Daegu", string.Empty },
        { "Busan", string.Empty },
        { "Daejeon", string.Empty },
        { "Gwangju", string.Empty },
        { "Cheongju", string.Empty }
            };

            while (true)
            {
                // 날씨 정보와 뉴스 정보를 비동기적으로 가져오기 
                List<string> weatherList = await GetWeatherByLocationAsync();
                List<string> newsList = await FetchNewsAsync();

                // 날씨 정보를 weatherArray에 업데이트하기
                for (int i = 0; i < weatherArray.GetLength(0); i++)
                {
                    weatherArray[i, 1] = weatherList[i];
                }

                int idx = 0;
                while (true)
                {
                    await Task.WhenAny<bool>
                    (
                        // 날씨 정보 레이아웃 숨기기 
                        news_info_layout.FadeTo(0, second / 2),
                        news_info_layout.TranslateTo(0, -50, second, Easing.Linear),

                        // 뉴스 정보 레이아웃 숨기기 
                        weather_info_layout.FadeTo(0, second / 2),
                        weather_info_layout.TranslateTo(0, -50, second, Easing.Linear)
                    );

                    // 다음 인덱스의 날씨와 뉴스 정보 화면에 표시 
                    idx++;
                    idx %= weatherArray.GetLength(0);
                    weather_data_label.Text = weatherArray[idx, 1];
                    news_label.Text = newsList[idx];

                    // 날씨 정보, 뉴스 정보 레이아웃 위치 초기화
                    await weather_info_layout.TranslateTo(0, 50, 0);
                    await news_info_layout.TranslateTo(0, 50, 0);


                    await Task.WhenAny<bool>
                    (
                        // 뉴스 정보 레이아웃 표시
                        news_info_layout.FadeTo(1, second / 2),
                        news_info_layout.TranslateTo(0, 0, second / 2, Easing.Linear),

                        // 날씨 정보 레이아웃 표시 
                        weather_info_layout.FadeTo(1, second / 2),
                        weather_info_layout.TranslateTo(0, 0, second / 2, Easing.Linear)
                    );

                    // 5초 동안 화면에 표시 후 다음 인덱스의 정보로 넘어가기 
                    await Task.Delay(5 * second);
                }
            }
        }
    }
}

