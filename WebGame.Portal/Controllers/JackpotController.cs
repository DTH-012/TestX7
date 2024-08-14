using System;
using System.Collections.Generic;
using System.Web.Http;
using MsWebGame.Portal.Database.DTO;
using MsWebGame.Portal.Database.DAO;

using MsWebGame.Portal.Handlers;
using System.Linq;
using Microsoft.Ajax.Utilities;
using MsWebGame.Portal.Models.RoomFunds;
using MsWebGame.RedisCache.Cache;
using TraditionGame.Utilities;
using CachingHandler = MsWebGame.Portal.Handlers.CachingHandler;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/Jackpot")]
    public class JackpotController : BaseApiController
    {
        /// <summary>
        /// jackport cho từng icon lớn trong game
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetJackpotTaiXiu")]
        [AllowAnonymous]
        public dynamic GetJackpotTaiXiu()
        {
            try
            {
                RedisCacheProvider _cachePvd = new RedisCacheProvider();
                long jackpot = _cachePvd.Get<long>("tx.session:info:Jackpot");
                return jackpot;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
        }


        /// <summary>
        /// jackport cho từng icon lớn trong game
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetJackpotBauCua")]
        [AllowAnonymous]
        public dynamic GetJackpotBauCua()
        {
            try
            {
                RedisCacheProvider _cachePvd = new RedisCacheProvider();
                long jackpot = _cachePvd.Get<long>("Game.BauCua:Jackpot");
                return jackpot;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
        }

        /// <summary>
        /// jackport cho từng icon lớn trong game
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetJackpotXocDia")]
        [AllowAnonymous]
        public dynamic GetJackpotXocDia()
        {
            try
            {
                RedisCacheProvider _cachePvd = new RedisCacheProvider();
                long jackpot = _cachePvd.Get<long>("Game.XocDia:Jackpot");
                return jackpot;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
        }

        // 1 2 3 4 15 16 17
        public List<TopJackPot> ModifyJackpotData(List<TopJackPot> listData)
        {
            long sum1 = listData.Where(c => c.RoomID == 2 && c.GameID == 12).Sum(c => c.JackpotFund);
            long sum2 = listData.Where(c => c.RoomID == 4 && c.GameID == 12).Sum(c => c.JackpotFund);
            listData.RemoveAll(c => c.GameID == 12 && (c.RoomID == 2 || c.RoomID == 4));
            listData.Add(new TopJackPot()
            {
                RoomID = 2,
                GameID = 12,
                JackpotFund = sum1
            });
            listData.Add(new TopJackPot()
            {
                RoomID = 4,
                GameID = 12,
                JackpotFund = sum2
            });

            int[] arr = new int[] { 1, 2, 3, 4, 15,16,17 };
            for (int i = 0; i < arr.Length; i++)
            {
                long sum3 = listData.Where(c => c.RoomID == 2 && c.GameID == arr[i]).Sum(c => c.JackpotFund);
                listData.RemoveAll(c => c.GameID == arr[i] && c.RoomID == 2);
                listData.Add(new TopJackPot()
                {
                    RoomID = 2,
                    GameID = arr[i],
                    JackpotFund = sum3
                });
            }

            return listData.OrderBy(c => c.RoomID).ThenByDescending(c => c.JackpotFund).ToList();
        }

        /// <summary>
        /// jackport cho từng icon lớn trong game
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetJackpotInfo")]
        [AllowAnonymous]
        public dynamic GetJackpotInfo()
        {
            try
            {
                //handler cache data
                var lstRs = CachingHandler.GetListCache<JackpotInfo>("JackpotAllCa", ServiceID);
                if (lstRs != null && lstRs.Count > 0)
                    return new { ResponseCode = 1, GameList = lstRs };


                List<JackpotInfo> listInfo = GetJpInfo();
                if (listInfo != null && listInfo.Count>0)
                {
                    NLogManager.LogMessage("GetJackpotInfo");
                    CachingHandler.AddListCache<JackpotInfo>("JackpotAllCa", ServiceID, listInfo, 5);
                    return new { ResponseCode = 1, GameList = listInfo };
                }
                else
                {
                    NLogManager.LogMessage("GetJackpotInfo Redis cache null");
                    lstRs = JackpotInfoDAO.Instance.GetGameJackPot();
                    if (lstRs == null)
                        lstRs = new List<JackpotInfo>();
                    //lstRs = ModifyJackpotData(lstRs);

                    CachingHandler.AddListCache<JackpotInfo>("JackpotAllCa", ServiceID, lstRs, 5);
                    return new { ResponseCode = 1, GameList = lstRs };
                }
                
            }
            catch
            {
                return new
                {
                    ResponseCode = 1,
                    list = new List<JackpotInfo>()
                };
            }
        }
        /// <summary>
        /// user jackport
        /// </summary>
        /// <returns></returns>
        [HttpOptions, HttpGet]
        [Route("GetUserJackpotInfo")]
        [AllowAnonymous]
        public dynamic GetUserJackpotInfo()
        {
            try
            {
                var lstRs = CachingHandler.GetListCache<UserJackPotInfo>("UserJackPotInfoCa", ServiceID);
                if (lstRs != null)
                {
                    return new
                    {
                        ResponseCode = 1,
                        list = lstRs,
                    };
                }

                var list = JackpotInfoDAO.Instance.GetUserJackportInfo();
                if (list == null)
                    list = new List<UserJackPotInfo>();
                CachingHandler.AddListCache<UserJackPotInfo>("UserJackPotInfoCa", ServiceID, list, 600);
                return new
                {
                    ResponseCode = 1,
                    list = list,
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    ResponseCode = 1,
                    list = new List<UserJackPotInfo>()
                };
            }
        }

        private List<JackpotInfo> GetJpInfo()
        {
            List<JackpotInfo> listjp = new List<JackpotInfo>();
            List<RoomFunds> temp = MiniStarKingdomJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new JackpotInfo()
                {
                    GameID = 7,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = MiniXPokerJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new JackpotInfo()
                {
                    GameID = 11,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = MiniBlockBusterJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new JackpotInfo()
                {
                    GameID = 12,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = SlotThreeKingdomJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new JackpotInfo()
                {
                    GameID = 2,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = SlotFrozenJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new JackpotInfo()
                {
                    GameID = 79,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = SlotThuyCungJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new JackpotInfo()
                {
                    GameID = 1,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = SlotSonGoKuJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new JackpotInfo()
                {
                    GameID = 15,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = SlotZombieJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new JackpotInfo()
                {
                    GameID = 83,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = SlotEgyptJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new JackpotInfo()
                {
                    GameID = 4,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = SlotGaiNhayJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new JackpotInfo()
                {
                    GameID = 17,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = SlotPandaJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new JackpotInfo()
                {
                    GameID = 80,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = SlotSuperHerdesJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new JackpotInfo()
                {
                    GameID = 81,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = SlotThanRungJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new JackpotInfo()
                {
                    GameID = 82,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = SlotCowboysJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new JackpotInfo()
                {
                    GameID = 3,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = SlotThanSetJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new JackpotInfo()
                {
                    GameID = 84,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            return listjp;
        }
        // GameID = 8
        private List<RoomFunds> TaiXiu()
        {
            List<RoomFunds> listCandy = new List<RoomFunds>();
            string keyHu = CachingHandler.GeneralRedisKey("tx.session:info:", "Jackpot");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            RoomFunds room = _cachePvd.Get<RoomFunds>(keyHu);
            NLogManager.LogMessage("room TaiXiu: " + room);
            return listCandy;

        }



        // GameID = 7
        private List<RoomFunds> MiniStarKingdomJackPot()
        {
            List<RoomFunds> listCandy = new List<RoomFunds>();
            for (int i = 0; i < 4; i++)
            {
                string keyHu = CachingHandler.GeneralRedisKey("KingStar", "RoomFunds" + (i + 1).ToString());
                RedisCacheProvider _cachePvd = new RedisCacheProvider();
                RoomFunds room = _cachePvd.Get<RoomFunds>(keyHu);
                if (room == null)
                {
                    NLogManager.LogMessage("MiniStarKingdomJackPot null: "+i);
                    return null;
                }
                else
                {
                    listCandy.Add(room);
                }
            }
            
            return listCandy;

        }
        // GameID = 11
        private List<RoomFunds> MiniXPokerJackPot()
        {
            List<RoomFunds> listCandy = new List<RoomFunds>();
            for (int i = 0; i < 3; i++)
            {
                string keyHu = CachingHandler.GeneralRedisKey("Xpoker", "RoomFunds" + (i + 1).ToString());
                RedisCacheProvider _cachePvd = new RedisCacheProvider();
                RoomFunds room = _cachePvd.Get<RoomFunds>(keyHu);
                if (room == null)
                {
                    NLogManager.LogMessage("MiniXPokerJackPot null: " + i);
                    return null;
                }
                else
                {
                    listCandy.Add(room);
                }
            }
            return listCandy;

        }
        // GameID = 12
        private List<RoomFunds> MiniBlockBusterJackPot()
        {
            List<RoomFunds> listCandy = new List<RoomFunds>();
            for (int i = 0; i < 5; i++)
            {
                string keyHu = CachingHandler.GeneralRedisKey("BlockBuster", "RoomFunds" + (i + 1).ToString());
                RedisCacheProvider _cachePvd = new RedisCacheProvider();
                RoomFunds room = _cachePvd.Get<RoomFunds>(keyHu);
                if (room == null)
                {
                    NLogManager.LogMessage("MiniBlockBusterJackPot null: " + i);
                    return null;
                }
                else
                {
                    listCandy.Add(room);
                }
            }
            return listCandy;

        }
        // GameID = 2
        private List<RoomFunds> SlotThreeKingdomJackPot()
        {
            List<RoomFunds> listCandy = new List<RoomFunds>();
            for (int i = 0; i < 4; i++)
            {
                string keyHu = CachingHandler.GeneralRedisKey("Tayduky", "RoomFunds" + (i + 1).ToString());
                RedisCacheProvider _cachePvd = new RedisCacheProvider();
                RoomFunds room = _cachePvd.Get<RoomFunds>(keyHu);
                if (room == null)
                {
                    NLogManager.LogMessage("SlotThreeKingdomJackPot null: " + i);
                    return null;
                }
                else
                {
                    listCandy.Add(room);
                }
            }
            return listCandy;

        }
        // GameID = 79
        private List<RoomFunds> SlotFrozenJackPot()
        {
            List<RoomFunds> listCandy = new List<RoomFunds>();
            for (int i = 0; i < 4; i++)
            {
                string keyHu = CachingHandler.GeneralRedisKey("Frozen", "RoomFunds" + (i + 1).ToString());
                RedisCacheProvider _cachePvd = new RedisCacheProvider();
                RoomFunds room = _cachePvd.Get<RoomFunds>(keyHu);
                if (room == null)
                {
                    NLogManager.LogMessage("SlotFrozenJackPot null: " + i);
                    return null;
                }
                else
                {
                    listCandy.Add(room);
                }
            }
            return listCandy;

        }
        // GameID = 1
        private List<RoomFunds> SlotThuyCungJackPot()
        {
            List<RoomFunds> listCandy = new List<RoomFunds>();
            for (int i = 0; i < 4; i++)
            {
                string keyHu = CachingHandler.GeneralRedisKey("Islands", "RoomFunds" + (i + 1).ToString());
                RedisCacheProvider _cachePvd = new RedisCacheProvider();
                RoomFunds room = _cachePvd.Get<RoomFunds>(keyHu);
                if (room == null)
                {
                    NLogManager.LogMessage("SlotThuyCungJackPot null: " + i);
                    return null;
                }
                    
                else
                {
                    listCandy.Add(room);
                }
            }
            return listCandy;

        }
        // GameId = 16
        

        // GameID = 15
        private List<RoomFunds> SlotSonGoKuJackPot()
        {
            List<RoomFunds> listCandy = new List<RoomFunds>();
            for (int i = 0; i < 4; i++)
            {
                string keyHu = CachingHandler.GeneralRedisKey("Songoku", "RoomFunds" + (i + 1).ToString());
                RedisCacheProvider _cachePvd = new RedisCacheProvider();
                RoomFunds room = _cachePvd.Get<RoomFunds>(keyHu);
                if (room == null)
                {
                    NLogManager.LogMessage("SlotSonGoKuJackPot null: " + i);
                    return null;
                }
                else
                {
                    listCandy.Add(room);
                }
            }
            return listCandy;

        }
        // GameID = 83
        private List<RoomFunds> SlotZombieJackPot()
        {
            List<RoomFunds> listCandy = new List<RoomFunds>();
            for (int i = 0; i < 4; i++)
            {
                string keyHu = CachingHandler.GeneralRedisKey("Zombie", "RoomFunds" + (i + 1).ToString());
                RedisCacheProvider _cachePvd = new RedisCacheProvider();
                RoomFunds room = _cachePvd.Get<RoomFunds>(keyHu);
                if (room == null)
                {
                    NLogManager.LogMessage("SlotZombieJackPot null: " + i);
                    return null;
                }
                else
                {
                    listCandy.Add(room);
                }
            }
            return listCandy;

        }
        // GameID = 4
        private List<RoomFunds> SlotEgyptJackPot()
        {
            List<RoomFunds> listCandy = new List<RoomFunds>();
            for (int i = 0; i < 4; i++)
            {
                string keyHu = CachingHandler.GeneralRedisKey("Egypt", "RoomFunds" + (i + 1).ToString());
                RedisCacheProvider _cachePvd = new RedisCacheProvider();
                RoomFunds room = _cachePvd.Get<RoomFunds>(keyHu);
                if (room == null)
                {
                    NLogManager.LogMessage("SlotEgyptJackPot null: " + i);
                    return null;
                }
                else
                {
                    listCandy.Add(room);
                }
            }
            return listCandy;

        }
        // GameID = 80
        private List<RoomFunds> SlotPandaJackPot()
        {
            List<RoomFunds> listCandy = new List<RoomFunds>();
            for (int i = 0; i < 4; i++)
            {
                string keyHu = CachingHandler.GeneralRedisKey("Panda", "RoomFunds" + (i + 1).ToString());
                RedisCacheProvider _cachePvd = new RedisCacheProvider();
                RoomFunds room = _cachePvd.Get<RoomFunds>(keyHu);
                if (room == null)
                {
                    NLogManager.LogMessage("SlotPandaJackPot null: " + i);
                    return null;
                }
                else
                {
                    listCandy.Add(room);
                }
            }
            return listCandy;

        }
        // GameID = 81
        private List<RoomFunds> SlotSuperHerdesJackPot()
        {
            List<RoomFunds> listCandy = new List<RoomFunds>();
            for (int i = 0; i < 4; i++)
            {
                string keyHu = CachingHandler.GeneralRedisKey("SuperHerdes", "RoomFunds" + (i + 1).ToString());
                RedisCacheProvider _cachePvd = new RedisCacheProvider();
                RoomFunds room = _cachePvd.Get<RoomFunds>(keyHu);
                if (room == null)
                {
                    NLogManager.LogMessage("SlotSuperHerdesJackPot null: " + i);
                    return null;
                }
                else
                {
                    listCandy.Add(room);
                }
            }
            return listCandy;

        }
        // GameID = 82
        private List<RoomFunds> SlotThanRungJackPot()
        {
            List<RoomFunds> listCandy = new List<RoomFunds>();
            for (int i = 0; i < 4; i++)
            {
                string keyHu = CachingHandler.GeneralRedisKey("ThanRung", "RoomFunds" + (i + 1).ToString());
                RedisCacheProvider _cachePvd = new RedisCacheProvider();
                RoomFunds room = _cachePvd.Get<RoomFunds>(keyHu);
                if (room == null)
                {
                    NLogManager.LogMessage("SlotThanRungJackPot null: " + i);
                    return null;
                }
                else
                {
                    listCandy.Add(room);
                }
            }
            return listCandy;

        }
        // GameID = 17
        private List<RoomFunds> SlotGaiNhayJackPot()
        {
            List<RoomFunds> listCandy = new List<RoomFunds>();
            for (int i = 0; i < 4; i++)
            {
                string keyHu = CachingHandler.GeneralRedisKey("GaiNhay", "RoomFunds" + (i + 1).ToString());
                RedisCacheProvider _cachePvd = new RedisCacheProvider();
                RoomFunds room = _cachePvd.Get<RoomFunds>(keyHu);
                if (room == null)
                {
                    NLogManager.LogMessage("SlotGaiNhayJackPot null: " + i);
                    return null;
                }
                else
                {
                    listCandy.Add(room);
                }
            }
            return listCandy;

        }
        // GameID = 3
        private List<RoomFunds> SlotCowboysJackPot()
        {
            List<RoomFunds> listCandy = new List<RoomFunds>();
            for (int i = 0; i < 4; i++)
            {
                string keyHu = CachingHandler.GeneralRedisKey("Cowboy", "RoomFunds" + (i + 1).ToString());
                RedisCacheProvider _cachePvd = new RedisCacheProvider();
                RoomFunds room = _cachePvd.Get<RoomFunds>(keyHu);
                if (room == null)
                {
                    NLogManager.LogMessage("SlotCowboysJackPot null: " + i);
                    return null;
                }
                else
                {
                    listCandy.Add(room);
                }
            }
            return listCandy;

        }
        // GameID = 84
        private List<RoomFunds> SlotThanSetJackPot()
        {
            List<RoomFunds> listCandy = new List<RoomFunds>();
            for (int i = 0; i < 4; i++)
            {
                string keyHu = CachingHandler.GeneralRedisKey("ThanSet", "RoomFunds" + (i + 1).ToString());
                RedisCacheProvider _cachePvd = new RedisCacheProvider();
                RoomFunds room = _cachePvd.Get<RoomFunds>(keyHu);
                if (room == null)
                {
                    //NLogManager.LogMessage("SlotCowboysJackPot null: " + i);
                    return null;
                }
                else
                {
                    listCandy.Add(room);
                }
            }
            return listCandy;

        }

        /// <summary>
        /// user jackport
        /// </summary>
        /// <returns></returns>
        [HttpOptions, HttpGet]
        [Route("GetTopJackPot")]
        [AllowAnonymous]
        public dynamic GetTopJackPot()
        {
            try
            {
                var lstRs = CachingHandler.GetListCache<TopJackPot>("GetTopJackPotList", ServiceID);
                if (lstRs != null && lstRs.Count > 0)
                {
                    return new
                    {
                        ResponseCode = 1,
                        list = lstRs,
                    };
                }

                var listTop = GetTopJpInfo();
                if (listTop != null)
                {
                    CachingHandler.AddListCache<TopJackPot>("GetTopJackPotList", ServiceID, listTop, 50);
                    return new
                    {
                        ResponseCode = 1,
                        list = listTop,
                    };
                }
                var list = JackpotInfoDAO.Instance.GetTopJackPot();
                if (list == null)
                    list = new List<TopJackPot>();
                else
                {
                    list = ModifyJackpotData(list);
                }
                CachingHandler.AddListCache<TopJackPot>("GetTopJackPotList", ServiceID, list, 50);
                return new
                {
                    ResponseCode = 1,
                    list = list,
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    ResponseCode = 1,
                    list = new List<UserJackPotInfo>()
                };
            }
        }

        private List<TopJackPot> GetTopJpInfo()
        {
            List<TopJackPot> listjp = new List<TopJackPot>();
            List<RoomFunds> temp = MiniStarKingdomJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new TopJackPot()
                {
                    GameID = 7,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = MiniXPokerJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new TopJackPot()
                {
                    GameID = 11,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = MiniBlockBusterJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                if (roomFundse.RoomID == 1)
                {
                    listjp.Add(new TopJackPot()
                    {
                        GameID = 12,
                        JackpotFund = roomFundse.JackpotFund,
                        RoomID = 2
                    });
                }
                else
                {
                    if (roomFundse.RoomID == 4)
                    {
                        listjp.Add(new TopJackPot()
                        {
                            GameID = 12,
                            JackpotFund = roomFundse.JackpotFund,
                            RoomID = 4
                        });
                    }
                }
                
            }
            temp = SlotThreeKingdomJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new TopJackPot()
                {
                    GameID = 2,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = SlotFrozenJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new TopJackPot()
                {
                    GameID = 79,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = SlotThuyCungJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new TopJackPot()
                {
                    GameID = 1,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            
            temp = SlotSonGoKuJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new TopJackPot()
                {
                    GameID = 15,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = SlotZombieJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new TopJackPot()
                {
                    GameID = 83,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = SlotEgyptJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new TopJackPot()
                {
                    GameID = 4,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = SlotGaiNhayJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new TopJackPot()
                {
                    GameID = 17,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = SlotCowboysJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new TopJackPot()
                {
                    GameID = 3,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = SlotThanSetJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new TopJackPot()
                {
                    GameID = 84,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = SlotThanRungJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new TopJackPot()
                {
                    GameID = 82,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = SlotSuperHerdesJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new TopJackPot()
                {
                    GameID = 81,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }
            temp = SlotPandaJackPot();
            if (temp == null)
            {
                return null;
            }
            foreach (var roomFundse in temp)
            {
                listjp.Add(new TopJackPot()
                {
                    GameID = 80,
                    JackpotFund = roomFundse.JackpotFund,
                    RoomID = (int)roomFundse.RoomID
                });
            }

            listjp.RemoveAll(c => c.RoomID == 3);
            listjp = listjp.OrderBy(c => c.RoomID).ThenByDescending(c => c.JackpotFund).ToList();
            return listjp;
        }

        [HttpGet]
        [Route("GetGameJackpotInfo")]
        [AllowAnonymous]
        public List<EventXJackpot> GetGameJackpotInfo()
        {
            try
            {
               
                var lstRs = CachingHandler.GetListCache<EventXJackpot>("EventXJackpotCa", ServiceID);
                if (lstRs != null && lstRs.Any())
                    return lstRs;

                lstRs = JackpotInfoDAO.Instance.GetEventXJackpot();
                if (lstRs == null)
                    lstRs = new List<EventXJackpot>();

                CachingHandler.AddListCache<EventXJackpot>("EventXJackpotCa", ServiceID, lstRs, 30);
                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
        }

        [HttpPost]
        [ActionName("GunEffectJackpot")]
        public bool GunEffectJackpot([FromBody] EffectJackpot input)
        {
            try
            {
                NLogManager.LogMessage(string.Format("EffectJackpot-NickName:{0}| JackpotValue:{1}| BetValue:{2}| GameName:{3}",
                   input.NickName, input.JackpotValue, input.BetValue, input.GameName));
                if (string.IsNullOrEmpty(input.NickName) || input.JackpotValue < 1 || input.BetValue < 1 || string.IsNullOrEmpty(input.GameName) || input.ServiceID <= 0)
                    return false;

                PortalHandler.Instance.GunEffectJackpot(input);
                return true;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return false;
            }
        }
    }
}
