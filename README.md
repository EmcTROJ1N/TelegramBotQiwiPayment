<br />
<div align="center">
  <img src="Icons/telegram.png" alt="Logo" width="80" height="80">
  <img src="Icons/Qiwi-icon.png" alt="Logo" width="80" height="80">

  <h3 align="center">Telegram payment bot</h3>

  <p align="center">
    <a href="https://github.com/EmcTROJ1N/TelegramBotQiwiPayment/">View Demo</a>
    ·
    <a href="https://github.com/EmcTROJ1N/TelegramBotQiwiPayment/issues">Report Bug</a>
    ·
    <a href="https://github.com/EmcTROJ1N/TelegramBotQiwiPayment/issues">Request Feature</a>
  </p>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

The Telegram Payment Bot is a powerful tool designed to enable users to make hassle-free payments for various services, including membership in private channels. With the increasing popularity of online transactions, 
it has become essential to have a secure platform where users can access and pay for various services with ease. The Telegram Payment Bot provides this convenience, allowing you to complete transactions quickly and
securely through the Telegram messaging app. Whether you're looking to subscribe to a premium channel or purchase exclusive content, the Telegram Payment Bot makes it easier than ever before to get what you want 
from your favorite creators and service providers.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Built With

The creation of the project involved:

| Technology                                                                                                            |
| ----------------------------------------------------------------------------------------------------------------------|
| ![QIWI](https://img.shields.io/badge/QIWI_api-orange?style=for-the-badge&logo=qiwi)
| ![DOTNET](https://img.shields.io/badge/C%23-DOTNET-blue?style=for-the-badge&logo=.Net)                                |
| ![TELEGRAM](https://img.shields.io/badge/TELEGRAM_api-blue?style=for-the-badge&logo=telegram)
<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- GETTING STARTED -->
## Getting Started

For this project to work, you need to meet some requirements:

<ol>
  <li>Your own telegram account</li>
  <li>.net framework (not .net core)</li>
  <li>Telegram bot api key</li>
  <li>Qiwi open and secret keys</li>
</ol>

<!-- ### Prerequisites

This is an example of how to list things you need to use the software and how to install them.
* npm
  ```sh
  npm install npm@latest -g
  ``` 
No special steps are necessary
-->

### Installation

Clone the repo
   ```sh
   git clone https://github.com/EmcTROJ1N/TelegramBotQiwiPayment
   ```

In the directory TelegramPaymentQiwiBot, create a file .env, in it type your api keys in this format:
```
QIWI_SECRET_KEY= *key*
QIWI_OPEN_KEY=*key*
TELEGRAM_BOT_TOKEN=*key*
```

Next, you need to configure the bot for your needs. Open the Program.cs file. It will contain several test offers. Each of your future offers is a separate descendant of BaseOffer class, which you need to fill 
here according to the suggested examples in the code. Also, it is necessary to set your data in the variables privateChanneld (this is the private channel's id to which the user will be added) and reportChannelId 
(this is the channel to which you will receive notifications about completed transactions). After that, the newly created objects must be placed in the offers collection.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

<ol>
  <li>Fork the Project</li>
  <li>Commit your Changes (`git commit -m 'Add some AmazingFeature'`)</li>
  <li>Push to the Branch (`git push origin feature/AmazingFeature`)</li>
  <li>Open a Pull Request</li>
</ol>

<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- CONTACT -->
## Contact

Your Name - [@emctroj1n](https://t.me/EmcTROJ1N) - 19et72@mail.ru

[Project Link](https://github.com/EmcTROJ1N/TelegramBotQiwiPayment)

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- ACKNOWLEDGMENTS -->
## Acknowledgments

Use this space to list resources you find helpful and would like to give credit to. I've included a few of my favorites to kick things off!

* [TelegramBots - .NET](https://github.com/TelegramBots)
* [QIWI-API/bill-payments](https://github.com/QIWI-API/bill-payments-dotnet-sdk)

<p align="right">(<a href="#readme-top">back to top</a>)</p>
