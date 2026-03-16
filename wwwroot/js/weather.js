async function loadWeather() {

    try {

        const response = await fetch('/Weather/GetWeather');
        const data = await response.json();

        const temp = document.getElementById("temp");
        const humidity = document.getElementById("humidity");
        const city = document.getElementById("city");
        const icon = document.getElementById("weatherIcon");

        if (!data || !data.main) {
            return;
        }

        temp.innerText = data.main.temp + " °C";
        humidity.innerText = data.main.humidity + "%";
        city.innerText = data.name;

        const iconCode = data.weather[0].icon;

        icon.src = "https://openweathermap.org/img/wn/" + iconCode + "@2x.png";

    }
    catch (error) {
        console.log("Weather loading failed", error);
    }
}

document.addEventListener("DOMContentLoaded", function () {
    loadWeather();
});