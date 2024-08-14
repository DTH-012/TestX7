using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Models.TiLeGame;
using MsWebGame.CSKH.Utils;
using MsWebGame.RedisCache.Cache;
using MsWebGame.CSKH.Models.Accounts;
using MsWebGame.CSKH.Models.LuckyDice;

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class TiLeGameController : BaseController
    {
        // GET: LuckyDice
        public ActionResult Index()
        {
            return View();
        }
        //TÀI XỈU
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetTaiXiu(DataPostSetTiLe data)
        {
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set("Game.TaiXiu:RateBet", data.Tile);
            return new JsonResult
            {
                Data = new
                {
                    Status = 1
                }
            };
        }
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetTaiXiu(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keyRateBet = "Game.TaiXiu:RateBet";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            
            return new JsonResult
            {
                Data = new
                {
                    rateBet = _cachePvd.Exists(keyRateBet) ? _cachePvd.Get<int>(keyRateBet) : 0
                },
            };
        }
        //TÀI XỈU MD5
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetTaiXiuMD5(DataPostSetTiLe data)
        {
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set("Game.TaiXiuMD5:RateBet", data.Tile);
            return new JsonResult
            {
                Data = new
                {
                    Status = 1
                }
            };
        }
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetTaiXiuMD5(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keyRateBet = "Game.TaiXiuMD5:RateBet";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();

            return new JsonResult
            {
                Data = new
                {
                    rateBet = _cachePvd.Exists(keyRateBet) ? _cachePvd.Get<int>(keyRateBet) : 0
                },
            };
        }
        //TÀI XỈU SIÊU TỐC
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetTaiXiuSIEUTOC(DataPostSetTiLe data)
        {
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set("Game.TaiXiuSIEUTOC:RateBet", data.Tile);
            return new JsonResult
            {
                Data = new
                {
                    Status = 1
                }
            };
        }
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetTaiXiuSIEUTOC(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keyRateBet = "Game.TaiXiuSIEUTOC:RateBet";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();

            return new JsonResult
            {
                Data = new
                {
                    rateBet = _cachePvd.Exists(keyRateBet) ? _cachePvd.Get<int>(keyRateBet) : 0
                },
            };
        }
        //BẦU CUA
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetBauCua(DataPostSetTiLe data)
        {
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set("Game.BauCua:RateBet", data.Tile);
            return new JsonResult
            {
                Data = new
                {
                    Status = 1
                }
            };
        }
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetBauCua(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keyRateBet = "Game.BauCua:RateBet";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();

            return new JsonResult
            {
                Data = new
                {
                    rateBet = _cachePvd.Exists(keyRateBet) ? _cachePvd.Get<int>(keyRateBet) : 0
                },
            };
        }
        //BẦU CUA JACKPOT
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetBauCuaJACKPOT(DataPostSetTiLe data)
        {
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set("Game.BauCua:RandomJackpot", data.Tile);
            return new JsonResult
            {
                Data = new
                {
                    Status = 1
                }
            };
        }
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetBauCuaJACKPOT(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keyRateBet = "Game.BauCua:RandomJackpot";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();

            return new JsonResult
            {
                Data = new
                {
                    rateBet = _cachePvd.Exists(keyRateBet) ? _cachePvd.Get<int>(keyRateBet) : 0
                },
            };
        }
        //XỐC ĐĨA
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetXocDia(DataPostSetTiLe data)
        {
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set("Game.XocDia:RateBet", data.Tile);
            return new JsonResult
            {
                Data = new
                {
                    Status = 1
                }
            };
        }
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetXocDia(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keyRateBet = "Game.XocDia:RateBet";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();

            return new JsonResult
            {
                Data = new
                {
                    rateBet = _cachePvd.Exists(keyRateBet) ? _cachePvd.Get<int>(keyRateBet) : 0
                },
            };
        }
        //XỐC ĐĨA JACKPOT
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetXocDiaJACKPOT(DataPostSetTiLe data)
        {
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set("Game.XocDia:RandomJackpot", data.Tile);
            return new JsonResult
            {
                Data = new
                {
                    Status = 1
                }
            };
        }
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetXocDiaJACKPOT(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keyRateBet = "Game.XocDia:RandomJackpot";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();

            return new JsonResult
            {
                Data = new
                {
                    rateBet = _cachePvd.Exists(keyRateBet) ? _cachePvd.Get<int>(keyRateBet) : 0
                },
            };
        }
        //RỒNG HỔ
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetRongHo(DataPostSetTiLe data)
        {
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set("Game.RongHo:RateBet", data.Tile);
            return new JsonResult
            {
                Data = new
                {
                    Status = 1
                }
            };
        }
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetRongHo(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keyRateBet = "Game.RongHo:RateBet";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();

            return new JsonResult
            {
                Data = new
                {
                    rateBet = _cachePvd.Exists(keyRateBet) ? _cachePvd.Get<int>(keyRateBet) : 0
                },
            };
        }
        //BACCARAT
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetBaccarat(DataPostSetTiLe data)
        {
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set("Game.Baccarat:RateBet", data.Tile);
            return new JsonResult
            {
                Data = new
                {
                    Status = 1
                }
            };
        }
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetBaccarat(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keyRateBet = "Game.Baccarat:RateBet";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();

            return new JsonResult
            {
                Data = new
                {
                    rateBet = _cachePvd.Exists(keyRateBet) ? _cachePvd.Get<int>(keyRateBet) : 0
                },
            };
        }
    }
}