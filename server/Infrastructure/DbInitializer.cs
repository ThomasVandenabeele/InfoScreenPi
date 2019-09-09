using Microsoft.EntityFrameworkCore;
using InfoScreenPi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace InfoScreenPi.Infrastructure
{
    public static class DbInitializer
    {
        private static InfoScreenContext context;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            context = (InfoScreenContext)serviceProvider.GetService(typeof(InfoScreenContext));

            InitializeItems("localhost"); //NAKIJKEN
            InitializeUserRoles();

        }

        private static void InitializeItems(string imagesPath)
        {
            if (!context.Items.Any())
            {
                context.Settings.Add(
                    new Setting
                    {
                        SettingName = "SlideTime",
                        SettingValue = "6000"
                    }
                );

                var _background1 = context.Backgrounds.Add(
                    new Background
                    {
                        Url = "http://s3.hbvlcdn.be/Assets/Images_Upload/2016/06/18/9061fe5a-356a-11e6-8c71-173369986151_web_scale_0.0607639_0.0607639__.jpg"
                    }
                ).Entity;

                var _background2 = context.Backgrounds.Add(
                    new Background
                    {
                        Url = "http://images.freepicturesweb.com/img1/05/02/26.jpg"
                    }
                ).Entity;

                var _background4 = context.Backgrounds.Add(
                    new Background
                    {
                        Url = "2722152.JPG"
                    }
                ).Entity;

                var _background5 = context.Backgrounds.Add(
                    new Background
                    {
                        Url = "92732393_zeeland.JPG"
                    }
                ).Entity;

                context.Backgrounds.Add(
                    new Background
                    {
                        Url = "050f37fb-6a46-4e9c-af17-fcefc5d8255d-1944680.jpg"
                    }
                );

                context.Backgrounds.Add(
                    new Background
                    {
                        Url = "6248795c-6ed8-436f-ac9b-1ae30c6f3d2d-29046177.jpg"
                    }
                );

                context.Backgrounds.Add(
                    new Background
                    {
                        Url = "c0452fa0-7765-412c-a139-2d631824b2c4-IMG_0853.0.jpg"
                    }
                );

                context.Backgrounds.Add(
                    new Background
                    {
                        Url = "fc1ac0a3-ce44-43cd-af4d-f8ff892795f8-IMG_4246.jpg"
                    }
                );

                context.Backgrounds.Add(
                    new Background
                    {
                        Url = "a48a5543-5db4-4553-9e57-944fe41f1130-kerk-centrum2.jpg"
                    }
                );

                context.Backgrounds.Add(
                    new Background
                    {
                        Url = "47f6ee6c-c04b-4b38-beb3-8abab9583766-1944587.jpg"
                    }
                );

                context.Backgrounds.Add(
                    new Background
                    {
                        Url = "5826178a-9fdb-4d2f-b97e-a889cc1b0e23-1944633.jpg"
                    }
                );

                context.Backgrounds.Add(
                    new Background
                    {
                        Url = "03c80a17-2e04-4964-a544-447b6ce95b32-126_001.jpg"
                    }
                );

                RssFeed _rssFeed1 = context.RssFeeds.Add(
                    new RssFeed
                    {
                        Description = "Het Belang van Limburg : Provincie Limburg",
                        Title = "Het Belang van Limburg : Limburg",
                        Source = "HBVL",
                        Active = false,
                        Url = "http://www.hbvl.be/rss/section/0DB351D4-B23C-47E4-AEEB-09CF7DD521F9",
                        PublicationDate = DateTime.Now,
                        StandardBackground = _background2
                    }
                ).Entity;

                context.Items.Add(
                    new RSSItem
                    {
                      RssFeed = _rssFeed1,
        	            Title = "Sint-Truiden trotseert gietende regen voor Rode Duivels",
        	            Content = "<p>In Sint-Truiden werd het een feestje om niet snel te vergeten. Ondanks de gietende regen zakten de fans massaal af om samen de overwinning van de Rode Duivels te vieren.</p>",
        	            Background = _background1,
        	            Active = true
                    }
                );

                context.Items.Add(
                    new CustomItem
                    {
                        Title = "Test item",
                        Content = "<center><h2><u>TEST</u> TEST</h2></center>",
                        Background = _background2,
                        Active = true,
                        Archieved = true
                    }
                );

                context.Items.Add(
                    new RSSItem
                    {
                        Title = "Vakantie Zomer",
                        Content = "<center><h2><span style='color:#ff0000'>Wegens jaarlijks verlof is de praktijk gesloten:</span></h2><h2><strong>van maandag 1 augustus 2016 tot en met maandag 15 augustus 2016.</strong></h2><p>&nbsp;</p><h3>U kan zich tijdens deze afwezigheid wenden tot de vervangende huisarts:</h3><h3>Dr. Michotte-Jansen</h3><h3>St.-Truidersteenweg 523</h3><h3>3500 St. - Lambrechts - Herk</h3><h3>Tel. 011/31.14.58</h3><p>&nbsp;</p><p><em>Steeds na telefonische afspraak !</em></p></center>",
                        Background = _background4,
                        Active = true
                    }
                );

                context.Items.Add(
                    new CustomItem
                    {
                        Title = "Zaterdag Gesloten",
                        Content = "<p style='text-align: center;'><strong>Tijdens de maanden juli en augustus is de praktijk op zaterdag gesloten.</strong></p><p style='text-align: center;'>Er kunnen op zaterdagvoormiddag GEEN afspraken gemaakt worden&nbsp;bij Dr. Vandenabeele.</p><p style='text-align: center;'>U kan zich op deze momenten wenden tot de Dokter van wacht, &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp</p><p style='text-align: center;'>op het nummer: 011/31.54.00.</p>",
                        Background = _background5,
                        Active = true,
                        Archieved = false
                    }
                );

                context.RssFeeds.Add(
                    new RssFeed
                    {
                        Description = "Recente artikels van gezondheid.be!",
                        Title = "Artikels op gezondheid.be",
                        Source = "GEZONDHEID",
                        Active = false,
                        Url = "http://www.gezondheid.be/rss/feeds/all.xml",
                        PublicationDate = DateTime.Now,
                        StandardBackground = _background2
                    }
                );

                context.SaveChanges();
            }
        }

        private static void InitializeUserRoles()
        {
            if (!context.Roles.Any())
            {
                // create roles
                context.Roles.AddRange(new Role[]
                {
                new Role()
                {
                    Name="Admin"
                }
                });

                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                context.Users.Add(new User()
                {
                    Email = "thomasvda@gmail.com",
                    Username = "TVDA",
                    FirstName = "Thomas",
                    LastName = "Vandenabeele",
                    HashedPassword = "9wsmLgYM5Gu4zA/BSpxK2GIBEWzqMPKs8wl2WDBzH/4=",
                    Salt = "GTtKxJA6xJuj3ifJtTXn9Q==",
                    IsLocked = false,
                    DateCreated = DateTime.Now,
                    LastLogin = DateTime.Now
                });

                // create user-admin for TVDA
                context.UserRoles.AddRange(new UserRole[] {
                new UserRole() {
                    RoleId = 1, // admin
                    UserId = 1  // TVDA
                }
            });
                context.SaveChanges();
            }
        }
    }
}
