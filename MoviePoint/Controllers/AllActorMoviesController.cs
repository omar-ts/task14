﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviePoint.Data;
using MoviePoint.Models;
using MoviePoint.Repos.IRepos;

namespace MoviePoint.Controllers
{
    public class AllActorMoviesController : Controller
    {
        IActorMovieRepo ActorMovieRepo;
        public AllActorMoviesController(IActorMovieRepo actorMovieRepo)
        {
            ActorMovieRepo = actorMovieRepo;
        }

        public IActionResult Index()
        {
            var actormovies = ActorMovieRepo.Get(Includation: [e => e.Movie, e => e.Actor]).ToList();
            return View(model: actormovies);
        }
        public IActionResult Create()
        {
            ViewBag.Movies = ActorMovieRepo.GetAll<Movie>().ToList();
            ViewBag.Actors = ActorMovieRepo.GetAll<Actor>().ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ActorMovie actormovie)
        {
            var existness = ActorMovieRepo.Get(filter: e => e.MovieId == actormovie.MovieId && e.ActorId == actormovie.ActorId);
            ViewBag.Movies = ActorMovieRepo.GetAll<Movie>().ToList();
            ViewBag.Actors = ActorMovieRepo.GetAll<Actor>().ToList();
            if (actormovie.MovieId <= 0)
            {
                ModelState.AddModelError("MovieId", "You should choose movie");
            }
            if (actormovie.ActorId <= 0)
            {
                ModelState.AddModelError("ActorId", "You should choose actor");
            }
            if (existness.Any())
            {
                ModelState.AddModelError(string.Empty, "This Actor has been in this movie before");
            }
            if (ModelState.IsValid)
            {
                ActorMovieRepo.Create(actormovie);
                ActorMovieRepo.Attemp();
                TempData["AMSuccess"] = "Actor Movie is successfully created";
                return RedirectToAction(nameof(Index));
            }
            return View(model: actormovie);
        }
        public IActionResult Edit(int Actorid, int Movieid)
        {
            var actormovie = ActorMovieRepo.GetOne(filter: e => e.ActorId == Actorid && e.MovieId == Movieid);
            ViewBag.Movies = ActorMovieRepo.GetAll<Movie>().ToList();
            ViewBag.Actors = ActorMovieRepo.GetAll<Actor>().ToList();
            return View(model: actormovie);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ActorMovie actormoviee, int oldactorid, int oldmovieid)
        {
            var actormovien = ActorMovieRepo.GetOne(filter: e => e.MovieId == oldmovieid && e.ActorId == oldactorid, tracked: false);
            var existness = ActorMovieRepo.Get(filter: e => e.MovieId == actormoviee.MovieId && e.ActorId == actormoviee.ActorId);
            if (existness.Any())
            {
                ModelState.AddModelError(string.Empty, "This Actor has been in this movie before");
            }
            ViewBag.Movies = ActorMovieRepo.GetAll<Movie>().ToList();
            ViewBag.Actors = ActorMovieRepo.GetAll<Actor>().ToList();
            if (ModelState.IsValid)
            {
                ActorMovieRepo.Delete(actormovien);
                ActorMovieRepo.Attemp();

                ActorMovieRepo.Create(actormoviee);
                ActorMovieRepo.Attemp();
                TempData["AMSuccess"] = "Actor Movie is successfully edited";
                return RedirectToAction(nameof(Index));
            }
            return View(model: actormoviee);
        }
        public IActionResult Delete(int Actorid, int Movieid)
        {
            var actormovieen = ActorMovieRepo.GetOne(filter: e => e.ActorId == Actorid && e.MovieId == Movieid);
            ActorMovieRepo.Delete(actormovieen);
            ActorMovieRepo.Attemp();
            TempData["AMSuccess"] = "Actor Movie is successfully deleted";
            return RedirectToAction(nameof(Index));
        }
    }
}
