﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using System.Net;
using System.Net.Http;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using JDPDO.SpaceBreeze;
using JDPDO.SpaceBreeze.UI.Models;
using JDPDO.SpaceBreeze.Extensions;

namespace JDPDO.SpaceBreeze.UI.Controllers
{
    public class OverviewController : Controller
    {
        readonly OverviewWindowModel model;
        private readonly IConfiguration _configuration;

        public OverviewController(IConfiguration configuration)
        {
            _configuration = configuration;
            model = new OverviewWindowModel(configuration);
        }

        public IActionResult Index()
        {
            return RedirectToAction("Uplinks");
        }

        #region uplinks
        /*
         * Containing API and view controller of uplinks section.
         */
        public IActionResult Uplinks()
        {
            return View(model.GetSubRegister("uplink"));
        }

        /// <summary>
        /// Adds an new uplink to instance register.
        /// </summary>
        /// <param name="type">Connection type of the new uplink.</param>
        /// <param name="title">Title of the new connection, works also as identifier.</param>
        /// <param name="host">Remote host for the new connection.</param>
        /// <param name="user">User of the new uplink.</param>
        /// <param name="password">Password of user of the new uplink.</param>
        /// <returns>Redirection to overviews uplinks site.</returns>
        [HttpPost("/api/uplink/add")]
        public IActionResult AddUplink(
            string type,
            string title,
            string host,
            string user,
            string password)
        {
            // Checks if instance already exists.
            if (model.ProvideInstance("uplink", title) == null)
            {
                // Create new uplink model instance and put to global instance register.
                UplinkModel uplink = new UplinkModel() { Host = host, Title = title, Type = type, User = user, Password = password };
                if (model.RegisterInstance("uplink", title, uplink)) return new OkResult();
                else return new BadRequestResult();
            }
            else return EditUplink(title, null, type, host, -1, user, password);
        }

        /// <summary>
        /// Edits an existing uplink.
        /// </summary>
        /// <param name="type">Type of the processed uplink.</param>
        /// <param name="title">Title of the uplink to be edited.</param>
        /// <param name="newTitle">Edited title of the uplink.</param>
        /// <param name="host">Edited host link.</param>
        /// <param name="port">Edited port.</param>
        /// <param name="user">Edited user.</param>
        /// <param name="password">Edited password of user.</param>
        /// <returns></returns>
        [HttpPost("/api/uplink/edit/{title}")]
        public IActionResult EditUplink(string title, string newTitle = null, string type = null, string host = null, int port = -1, string user = null, string password = null)
        {
            UplinkModel uplink = (UplinkModel)model.ProvideInstance("uplink", title);
            if (uplink != null)
            {
                /* Assign new properties. */
                // type
                if (!String.IsNullOrWhiteSpace(type)) uplink.Type = type;
                // title
                if (!String.IsNullOrWhiteSpace(newTitle)) uplink.Title = newTitle;
                // host
                if (!String.IsNullOrWhiteSpace(host)) uplink.Host = host;
                // port
                if (port != -1) uplink.Host = new UriBuilder(uplink.Type, uplink.Host, port).Path;
                // user
                if (!String.IsNullOrWhiteSpace(user)) uplink.User = user;
                // password
                if (!String.IsNullOrWhiteSpace(password)) uplink.Password = password;

                // Register edited uplink and delete old instance.
                if (model.RemoveInstance(uplink.Type, title)) model.RegisterInstance(uplink.Type, uplink.Title, uplink);

                return Ok();
            }
            return BadRequest();
        }

        /// <summary>
        /// Removes an existing uplink from registers.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        [Route("/api/uplink/remove/{title}")]
        public IActionResult RemoveUplink(string title)
        {
            return model.RemoveInstance("uplink", title) ? Ok() : (StatusCodeResult) BadRequest();
        }

        [Route("/api/uplink/open/{title}")]
        public async Task<IActionResult> OpenUplink(string title)
        {
            bool success = await model.OpenUplinkWindowAsync(title);
            return (success) ? Ok() : StatusCode(500);
        }

        #endregion

        #region Settings
        public IActionResult Settings()
        {
            return View();
        }
        #endregion
    }
}
