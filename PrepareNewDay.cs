using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace igotnousername.PrepareNewDay {

    public class PrepareNewDay : Mod {

        private Config Config;
        private int LastTimeInterval { get; set; }
        private bool isAfterCutOffTime = false;
        private bool wentOutside = false;
        
        public override void Entry(IModHelper helper) {
            Config = helper.ReadConfig<Config>();
            helper.Events.Player.Warped += Player_Warped;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;

        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e) {
            LastTimeInterval = Game1.gameTimeInterval;
            isAfterCutOffTime = false;
            wentOutside = false;
        }

        private void Player_Warped(object sender, WarpedEventArgs e) {
            if (ShouldTimeFreeze(e.NewLocation)) {
                if (Game1.timeOfDay >= Config.cutOffTime) {
                    LastTimeInterval = Game1.gameTimeInterval;
                }
                else {
                    if (wentOutside) {
                        LastTimeInterval = -1;
                    }
                    else {
                        LastTimeInterval = Game1.gameTimeInterval;
                    }
                }
                return;
            }

            LastTimeInterval = -1;
            wentOutside = true;
        }


        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e) {
            if (!Context.IsWorldReady) return;

            if (Game1.timeOfDay >= Config.cutOffTime && !isAfterCutOffTime) {
                if (ShouldTimeFreeze(Game1.player.currentLocation)) {
                    LastTimeInterval = Game1.gameTimeInterval;
                }
                isAfterCutOffTime = true;
            }

            if (LastTimeInterval >= 0) {
                Game1.gameTimeInterval = LastTimeInterval;
            }
        }

        private bool ShouldTimeFreeze(GameLocation location) {
            if (location.Name == Config.houseName) {
                return true;
            }

            if (location.Name == "Cellar" && Config.isCellarIncluded) {
                return true;
            }
            
            return false;
        }
    }
}
