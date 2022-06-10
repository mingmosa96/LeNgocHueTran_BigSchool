﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LeNgocHueTran_BigSchool.Models;
using Microsoft.AspNet.Identity;

namespace LeNgocHueTran_BigSchool.Controllers
{
    public class FollowController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Follow(Following follow)
        {
            var userID = User.Identity.GetUserId();
            if (userID == null)
            {
                return BadRequest("Please login first!");
            }
            if (userID == follow.FolloweeId)
            {
                return BadRequest("Can not follow myself!");
            }
            BigSchoolContext context = new BigSchoolContext();
            Following find = context.Following.FirstOrDefault(p => p.FollowerId == userID && p.FolloweeId == follow.FolloweeId);
            if (find != null)
            {
                context.Following.Remove(context.Following.SingleOrDefault(
                    p=> p.FollowerId == userID && p.FolloweeId == follow.FolloweeId));
                context.SaveChanges();
                return Ok("cancel");
            }

            follow.FollowerId = userID;
            context.Following.Add(follow);
            context.SaveChanges();

            return Ok();
        }
    }
}
