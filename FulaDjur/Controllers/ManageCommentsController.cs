using FulaDjur.Data;
using FulaDjur.Data.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FulaDjur.Controllers
{
    [Authorize]
    public class ManageCommentsController : Controller
    {

        IUglyCommentRepository _comments;

        public ManageCommentsController()
        {
            _comments = new UglyCommentRepository();
        }

        // GET: ManageComments
        public ActionResult Index()
        {
            var comments = _comments.GetAll(null);

            return View(comments);
        }

        public ActionResult Delete(string ImageUri)
        {
            _comments.Delete(ImageUri);

            return RedirectToAction("Index");
        }
    }
}