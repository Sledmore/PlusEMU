using System;
using System.Threading;
using log4net;
using Plus.Communication.Packets.Outgoing.Handshake;
using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Users.Process
{
    sealed class ProcessComponent
    {
        private static readonly ILog log = LogManager.GetLogger("Plus.HabboHotel.Users.Process.ProcessComponent");

        /// <summary>
        /// Player to update, handle, change etc.
        /// </summary>
        private Habbo _player = null;

        /// <summary>
        /// ThreadPooled Timer.
        /// </summary>
        private Timer _timer = null;

        /// <summary>
        /// Prevents the timer from overlapping itself.
        /// </summary>
        private bool _timerRunning = false;

#pragma warning disable CS0414 // The field 'ProcessComponent._timerLagging' is assigned but its value is never used
        /// <summary>
        /// Checks if the timer is lagging behind (server can't keep up).
        /// </summary>
        private bool _timerLagging = false;
#pragma warning restore CS0414 // The field 'ProcessComponent._timerLagging' is assigned but its value is never used

        /// <summary>
        /// Enable/Disable the timer WITHOUT disabling the timer itself.
        /// </summary>
        private bool _disabled = false;

        /// <summary>
        /// Used for disposing the ProcessComponent safely.
        /// </summary>
        private AutoResetEvent _resetEvent = new AutoResetEvent(true);

        /// <summary>
        /// How often the timer should execute.
        /// </summary>
        private static int _runtimeInSec = 60;

        /// <summary>
        /// Default.
        /// </summary>
        public ProcessComponent()
        {
        }

        /// <summary>
        /// Initializes the ProcessComponent.
        /// </summary>
        /// <param name="Player">Player.</param>
        public bool Init(Habbo Player)
        {
            if (Player == null)
                return false;
            else if (_player != null)
                return false;

            _player = Player;
            _timer = new Timer(new TimerCallback(Run), null, _runtimeInSec * 1000, _runtimeInSec * 1000);
            return true;
        }

        /// <summary>
        /// Called for each time the timer ticks.
        /// </summary>
        /// <param name="State"></param>
        public void Run(object State)
        {
            try
            {
                if (_disabled)
                    return;

                if (_timerRunning)
                {
                    _timerLagging = true;
                    log.Warn("<Player " + _player.Id + "> Server can't keep up, Player timer is lagging behind.");
                    return;
                }

                _resetEvent.Reset();

                // BEGIN CODE

                #region Muted Checks
                if (_player.TimeMuted > 0)
                    _player.TimeMuted -= 60;
                #endregion

                #region Console Checks
                if (_player.MessengerSpamTime > 0)
                    _player.MessengerSpamTime -= 60;
                if (_player.MessengerSpamTime <= 0)
                    _player.MessengerSpamCount = 0;
                #endregion

                _player.TimeAFK += 1;

                #region Respect checking
                if (_player.GetStats().RespectsTimestamp != DateTime.Today.ToString("MM/dd"))
                {
                    _player.GetStats().RespectsTimestamp = DateTime.Today.ToString("MM/dd");
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `user_stats` SET `dailyRespectPoints` = '" + (_player.Rank == 1 && _player.VIPRank == 0 ? 10 : _player.VIPRank == 1 ? 15 : 20) + "', `dailyPetRespectPoints` = '" + (_player.Rank == 1 && _player.VIPRank == 0 ? 10 : _player.VIPRank == 1 ? 15 : 20) + "', `respectsTimestamp` = '" + DateTime.Today.ToString("MM/dd") + "' WHERE `id` = '" + _player.Id + "' LIMIT 1");
                    }

                    _player.GetStats().DailyRespectPoints = (_player.Rank == 1 && _player.VIPRank == 0 ? 10 : _player.VIPRank == 1 ? 15 : 20);
                    _player.GetStats().DailyPetRespectPoints = (_player.Rank == 1 && _player.VIPRank == 0 ? 10 : _player.VIPRank == 1 ? 15 : 20);

                    if (_player.GetClient() != null)
                    {
                        _player.GetClient().SendPacket(new UserObjectComposer(_player));
                    }
                }
                #endregion

                #region Reset Scripting Warnings
                if (_player.GiftPurchasingWarnings < 15)
                    _player.GiftPurchasingWarnings = 0;

                if (_player.MottoUpdateWarnings < 15)
                    _player.MottoUpdateWarnings = 0;

                if (_player.ClothingUpdateWarnings < 15)
                    _player.ClothingUpdateWarnings = 0;
                #endregion


                if (_player.GetClient() != null)
                    PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(_player.GetClient(), "ACH_AllTimeHotelPresence", 1);

                _player.CheckCreditsTimer();
                _player.Effects().CheckEffectExpiry(_player);

                // END CODE

                // Reset the values
                _timerRunning = false;
                _timerLagging = false;

                _resetEvent.Set();
            }
            catch { }
        }

        /// <summary>
        /// Stops the timer and disposes everything.
        /// </summary>
        public void Dispose()
        {
            // Wait until any processing is complete first.
            try
            {
                _resetEvent.WaitOne(TimeSpan.FromMinutes(5));
            }
            catch { } // give up

            // Set the timer to disabled
            _disabled = true;

            // Dispose the timer to disable it.
            try
            {
                if (_timer != null)
                    _timer.Dispose();
            }
            catch { }

            // Remove reference to the timer.
            _timer = null;

            // Null the player so we don't reference it here anymore
            _player = null;
        }
    }
}