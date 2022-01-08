using EnvyLogging.Component;
using EnvyLogging.Configuration;
using EnvyLogging.Database;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Core.Utils;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger = Rocket.Core.Logging.Logger;

namespace EnvyLogging
{
    public class EnvyLoggingPlugin : RocketPlugin<EnvyLoggingConfiguration>
    {
        public static EnvyLoggingPlugin Instance { get; set; }
        public DatabaseManager databaseManager { get; set; }
        protected override void Load()
        {
            Instance = this;
            Logger.Log("Plugin loaded succefully");
            databaseManager = new DatabaseManager(); databaseManager.Reload();
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            EffectManager.onEffectButtonClicked += OnEffectButtonClicked;
            EffectManager.onEffectTextCommitted += OnEffectTextCommited;


        }

        private void OnEffectTextCommited(Player caller, string buttonName, string text)
        {
         UnturnedPlayer player = UnturnedPlayer.FromPlayer(caller);

            var component = player.GetComponent<EnvyLoggingComponent>();


            switch (buttonName)
            {
                case "InputLastname":
                    component.Lastname = text;
                    break;
                case "InputNombre":
                    component.Name = text;
                    break;
                case "InputEdad":
                    int.TryParse(text, out int value);

                    if(value > Configuration.Instance.MaximaEdad)
                    {
                        EffectManager.sendUIEffectText(13513, player.SteamPlayer().transportConnection, true, "TextError", "La edad maxima es de " + Configuration.Instance.MaximaEdad);
                    }
                    else if(value < Configuration.Instance.MinimaEdad)
                    {
                        EffectManager.sendUIEffectText(13513, player.SteamPlayer().transportConnection, true, "TextError", "La edad minima es de " + Configuration.Instance.MinimaEdad);
                    }
                    else
                    {
                        component.Year = value;
                        EffectManager.sendUIEffectText(13513, player.SteamPlayer().transportConnection, true, "TextError", "");
                    }
                    
                    break;
            }
        }

        private void OnEffectButtonClicked(Player caller, string buttonName)
        {
            UnturnedPlayer player = UnturnedPlayer.FromPlayer(caller);
            var component = player.GetComponent<EnvyLoggingComponent>();
            switch (buttonName)
            {
                case "ButtonEntrar":
                    EffectManager.sendUIEffectVisibility(13513, player.SteamPlayer().transportConnection, true, "Loggin", false);

                    EffectManager.sendUIEffectVisibility(13513, player.SteamPlayer().transportConnection, true, "LoginUser", true);



                    break;
                case "ButtonReglas":
                    EffectManager.sendUIEffectVisibility(13513, player.SteamPlayer().transportConnection, true, "Loggin", false);
                    EffectManager.sendUIEffectVisibility(13513, player.SteamPlayer().transportConnection, true, "LoginReglas", true);

                    TaskDispatcher.QueueOnMainThread(() =>
                    {
                        string reglas = string.Empty;
                        foreach (string item in Configuration.Instance.Reglas)
                        {
                            reglas += item + System.Environment.NewLine;
                        }
                        EffectManager.sendUIEffectText(13513, player.SteamPlayer().transportConnection, true, "TextReglas", reglas);
                    });
                    
                    break;
                case "ButtonMan":
                    component.Sexo = "masculino";
                    break;
                case "ButtonGirl":
                    component.Sexo = "femenino";
                    break;
                case "ButtonDiscord":
                    player.Player.sendBrowserRequest("Discord", Configuration.Instance.DiscordLink);
                    break;
                case "ButtonCloseLoginUser":
                    EffectManager.sendUIEffectVisibility(13513, player.SteamPlayer().transportConnection, true, "Loggin", true);
                    EffectManager.sendUIEffectVisibility(13513, player.SteamPlayer().transportConnection, true, "LoginUser", false);
                    break;
                case "BUTTON_CLOSE":
                    EffectManager.sendUIEffectVisibility(13513, player.SteamPlayer().transportConnection, true, "Loggin", true);
                    EffectManager.sendUIEffectVisibility(13513, player.SteamPlayer().transportConnection, true, "LoginReglas", false);
                    break;
                case "Button":

                    
                    if(component.Name == null || component.Name == "")
                    {
                        EffectManager.sendUIEffectText(13513, player.SteamPlayer().transportConnection, true, "TextError", "No ingresaste el nombre");
                    }
                    else if(component.Lastname == null || component.Lastname == "")
                    {
                        EffectManager.sendUIEffectText(13513, player.SteamPlayer().transportConnection, true, "TextError", "No ingresaste el apellido");
                    }
                    else if(component.Year <= 0)
                    {
                        EffectManager.sendUIEffectText(13513, player.SteamPlayer().transportConnection, true, "TextError", "No ingresaste la edad");
                    }
                    else if(component.Sexo == null || component.Sexo == "")
                    {
                        EffectManager.sendUIEffectText(13513, player.SteamPlayer().transportConnection, true, "TextError", "No ingresaste el sexo");
                    }
                    else
                    {
                        databaseManager.AddAccount(player.CSteamID, component.Name, component.Lastname, component.Sexo, component.Year);
                        player.Player.disablePluginWidgetFlag(EPluginWidgetFlags.Modal);
                        EffectManager.askEffectClearByID(13513, player.SteamPlayer().transportConnection);
                        EffectManager.askEffectClearByID(13512, player.SteamPlayer().transportConnection);

                        player.Player.enablePluginWidgetFlag(EPluginWidgetFlags.ShowHealth);
                        player.Player.enablePluginWidgetFlag(EPluginWidgetFlags.ShowFood);
                        player.Player.enablePluginWidgetFlag(EPluginWidgetFlags.ShowOxygen);
                        player.Player.enablePluginWidgetFlag(EPluginWidgetFlags.ShowStamina);
                        player.Player.enablePluginWidgetFlag(EPluginWidgetFlags.ShowVirus);
                        player.Player.enablePluginWidgetFlag(EPluginWidgetFlags.ShowWater);
                    }
                    break;
            }

        }

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            try
            {
                var data = databaseManager.GetAccount(player.CSteamID);
                if (data == null) throw new Exception();
            }
            catch
            {
                player.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowHealth);
                player.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowFood);
                player.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowOxygen);
                player.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowStamina);
                player.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowVirus);
                player.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowWater);
                player.Player.enablePluginWidgetFlag(EPluginWidgetFlags.Modal);
                EffectManager.sendUIEffect(13512, 13513, player.SteamPlayer().transportConnection, true);
                EffectManager.sendUIEffectImageURL(13513, player.SteamPlayer().transportConnection, true, "WALLPAPER", Configuration.Instance.WallpaperImage, true, false);
                EffectManager.sendUIEffectText(13513, player.SteamPlayer().transportConnection, true, "ButtonEntrarText", Translate("Button:Iniciar"));
                EffectManager.sendUIEffectText(13513, player.SteamPlayer().transportConnection, true, "ButtonReglasText", Translate("Button:Reglas"));
                EffectManager.sendUIEffectText(13513, player.SteamPlayer().transportConnection, true, "TextName", Translate("Text:Name"));
                EffectManager.sendUIEffectText(13513, player.SteamPlayer().transportConnection, true, "TextYear", Translate("Text:Year"));
                EffectManager.sendUIEffectText(13513, player.SteamPlayer().transportConnection, true, "TextLastname", Translate("Text:Lastname"));
                EffectManager.sendUIEffectText(13513, player.SteamPlayer().transportConnection, true, "TextSex", Translate("Text:Sex"));
                EffectManager.sendUIEffectVisibility(13513, player.SteamPlayer().transportConnection, true, "Loggin", true);

                if (Configuration.Instance.Music)
                {
                    EffectManager.sendUIEffectVisibility(13513, player.SteamPlayer().transportConnection, true, "music", true);
                }
            }
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                TranslationList translationList = new TranslationList();
                translationList.Add("Button:Iniciar", "Iniciar");
                translationList.Add("Button:Reglas", "Reglas");
                translationList.Add("Text:Name", "Nombre:");
                translationList.Add("Text:Year", "Edad:");
                translationList.Add("Text:Lastname", "Apellido:");
                translationList.Add("Text:Sex", "Sexo:");
                return translationList;
            }
        }

        protected override void Unload()
        {
            Logger.Log("plugin unloaded");
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            EffectManager.onEffectButtonClicked -= OnEffectButtonClicked;
            EffectManager.onEffectTextCommitted -= OnEffectTextCommited;
        }
    }
}
