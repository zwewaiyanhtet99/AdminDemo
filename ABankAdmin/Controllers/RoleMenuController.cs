using ABankAdmin.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ABankAdmin.Controllers
{
    public class RoleMenuController : Controller
    {
        private AdminDBContext db = new AdminDBContext();
        string controllerName = "RoleMenuController";
        EventLogController log = new EventLogController();
        // GET: RoleMenu
        public ActionResult Index(int? id)
        {
            try
            {
                List<Role> roles = db._Roles.Where(rt => rt.Name != "Administrator").ToList();
                var roleid = db._Roles.Where(rt => rt.Name != "Administrator").FirstOrDefault().ID;
                if (id == null) { id = roleid; }

                List<TreeViewNode> nodes = new List<TreeViewNode>();
                List<Menu> menus = db.Menus.ToList();

                //get current role menus
                IEnumerable<Menu> rolemenus = from m in db.Menus
                                              join rm in db.RoleMenus on m.ID equals rm.MenuID into tblmenus
                                              from tblrm in tblmenus.ToList()
                                              where m.Active == true && tblrm.RoleID == id
                                              select m;

                //Loop and add the Parent Nodes.
                foreach (Menu mn in menus)
                {
                    if (mn.Parent_ID == 0)
                        nodes.Add(new TreeViewNode { id = mn.ID.ToString(), parent = "#", text = mn.Menu_Name, state = new state { selected = false } });
                    else
                    {
                        Boolean chk = (rolemenus.Where(rm => rm.ID == mn.ID).ToList().Count() > 0);
                        nodes.Add(new TreeViewNode { id = mn.ID.ToString(), parent = mn.Parent_ID.ToString(), text = mn.Menu_Name, state = new state { selected = chk } });
                    }
                }
                //curry current role
                Role r = db._Roles.Find(id);
                ViewBag.rolename = r.Name;
                ViewBag.role = id;
                //Serialize to JSON string.
                ViewBag.Json = (new JavaScriptSerializer()).Serialize(nodes);
                if (TempData["message"] != null && TempData["message"].ToString() != "")
                ViewBag.message = TempData["message"];
                return View(roles);
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, "Index", ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Index(string selectedItems, string role)
        {
            try
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                int roleid = Convert.ToInt32(role);
                if (roleid == 3)//skid if Administrator
                {
                    return RedirectToAction("Index");
                }
                //delete old records
                db.RoleMenus.RemoveRange(db.RoleMenus.Where(rm => rm.RoleID == roleid));
                List<TreeViewNode> nodes = (new JavaScriptSerializer()).Deserialize<List<TreeViewNode>>(selectedItems);
                foreach (var n in nodes) 
                {
                    RoleMenu rm = new RoleMenu();
                    rm.RoleID = roleid;
                    rm.MenuID = Convert.ToInt32(n.id);
                    db.RoleMenus.Add(rm);
                    db.SaveChanges();
                    // save log
                    log.Info(User.Identity.GetUserId(),controllerName, actionName, 2);
                }
            }
            catch (Exception ex)
            {
                log.Error(User.Identity.GetUserId(),controllerName, "Index", ex);
                return View("Error");
            }
                TempData["message"] = "Update Successfully.";
            return RedirectToAction("Index");
        }
    }
}