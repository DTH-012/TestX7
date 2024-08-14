using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using MsWebGame.CSKH.App_Start;
using MsWebGame.CSKH.Database.DAO;
using MsWebGame.CSKH.Models.TiLeGameSlot;
using MsWebGame.CSKH.Utils;
using MsWebGame.RedisCache.Cache;
using MsWebGame.CSKH.Models.Accounts;
using MsWebGame.CSKH.Models.LuckyDice;

namespace MsWebGame.CSKH.Controllers
{
    [AllowedIP]
    public class TiLeGameSlotController : BaseController
    {
        // GET: LuckyDice
        public ActionResult Index()
        {
            return View();
        }
        //COWBOY
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetCowboy(DataPostSetTiLe data)
        {
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set("Game.Cowboy:TyLe", data.Tile);
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
        public ActionResult GetCowboy(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keyRateBet = "Game.Cowboy:TyLe";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            
            return new JsonResult
            {
                Data = new
                {
                    rateBet = _cachePvd.Exists(keyRateBet) ? _cachePvd.Get<int>(keyRateBet) : 0
                },
            };
        }
        //THUỶ CUNG
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetThuyCung(DataPostSetTiLe data)
        {
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set("Game.Islands:TyLe", data.Tile);
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
        public ActionResult GetThuyCung(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keyRateBet = "Game.Islands:TyLe";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();

            return new JsonResult
            {
                Data = new
                {
                    rateBet = _cachePvd.Exists(keyRateBet) ? _cachePvd.Get<int>(keyRateBet) : 0
                },
            };
        }
        //AI CẬP
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetAiCap(DataPostSetTiLe data)
        {
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set("Game.Egypt:TyLe", data.Tile);
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
        public ActionResult GetAiCap(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keyRateBet = "Game.Egypt:TyLe";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();

            return new JsonResult
            {
                Data = new
                {
                    rateBet = _cachePvd.Exists(keyRateBet) ? _cachePvd.Get<int>(keyRateBet) : 0
                },
            };
        }
        //FROZEN
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetFrozen(DataPostSetTiLe data)
        {
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set("Game.Frozen:TyLe", data.Tile);
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
        public ActionResult GetFrozen(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keyRateBet = "Game.Frozen:TyLe";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();

            return new JsonResult
            {
                Data = new
                {
                    rateBet = _cachePvd.Exists(keyRateBet) ? _cachePvd.Get<int>(keyRateBet) : 0
                },
            };
        }
        //Gái Nhảy
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetGaiNhay(DataPostSetTiLe data)
        {
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set("Game.GaiNhay:TyLe", data.Tile);
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
        public ActionResult GetGaiNhay(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keyRateBet = "Game.GaiNhay:TyLe";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();

            return new JsonResult
            {
                Data = new
                {
                    rateBet = _cachePvd.Exists(keyRateBet) ? _cachePvd.Get<int>(keyRateBet) : 0
                },
            };
        }
        //Panda
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetPanda(DataPostSetTiLe data)
        {
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set("Game.Panda:TyLe", data.Tile);
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
        public ActionResult GetPanda(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keyRateBet = "Game.Panda:TyLe";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();

            return new JsonResult
            {
                Data = new
                {
                    rateBet = _cachePvd.Exists(keyRateBet) ? _cachePvd.Get<int>(keyRateBet) : 0
                },
            };
        }
        //Songuku
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetSongoku(DataPostSetTiLe data)
        {
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set("Game.Songoku:TyLe", data.Tile);
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
        public ActionResult GetSongoku(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keyRateBet = "Game.Songoku:TyLe";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();

            return new JsonResult
            {
                Data = new
                {
                    rateBet = _cachePvd.Exists(keyRateBet) ? _cachePvd.Get<int>(keyRateBet) : 0
                },
            };
        }
        //SuperHerdes
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetSuperHerdes(DataPostSetTiLe data)
        {
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set("Game.SuperHerdes:TyLe", data.Tile);
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
        public ActionResult GetSuperHerdes(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keyRateBet = "Game.SuperHerdes:TyLe";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();

            return new JsonResult
            {
                Data = new
                {
                    rateBet = _cachePvd.Exists(keyRateBet) ? _cachePvd.Get<int>(keyRateBet) : 0
                },
            };
        }
        //THẦN RỪNG
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetThanRung(DataPostSetTiLe data)
        {
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set("Game.ThanRung:TyLe", data.Tile);
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
        public ActionResult GetThanRung(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keyRateBet = "Game.ThanRung:TyLe";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();

            return new JsonResult
            {
                Data = new
                {
                    rateBet = _cachePvd.Exists(keyRateBet) ? _cachePvd.Get<int>(keyRateBet) : 0
                },
            };
        }
        //THẦN SÉT
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetThanSet(DataPostSetTiLe data)
        {
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set("Game.ThanSet:TyLe", data.Tile);
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
        public ActionResult GetThanSet(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keyRateBet = "Game.ThanSet:TyLe";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();

            return new JsonResult
            {
                Data = new
                {
                    rateBet = _cachePvd.Exists(keyRateBet) ? _cachePvd.Get<int>(keyRateBet) : 0
                },
            };
        }
        //ZOMBIE
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetZombie(DataPostSetTiLe data)
        {
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set("Game.Zombie:TyLe", data.Tile);
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
        public ActionResult GetZombie(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keyRateBet = "Game.Zombie:TyLe";

            RedisCacheProvider _cachePvd = new RedisCacheProvider();

            return new JsonResult
            {
                Data = new
                {
                    rateBet = _cachePvd.Exists(keyRateBet) ? _cachePvd.Get<int>(keyRateBet) : 0
                },
            };
        }
        //TAM QUỐC
        [AdminAuthorize(Roles = ADMIN_MONITOR_ROLE)]
        [HttpPost]
        public ActionResult SetTamQuoc(DataPostSetTiLe data)
        {
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            _cachePvd.Set("Game.Tayduky:TyLe", data.Tile);
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
        public ActionResult GetTamQuoc(GridCommand command)
        {
            //lay danh sách chăm sóc khách hàng
            string keyRateBet = "Game.Tayduky:TyLe";

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