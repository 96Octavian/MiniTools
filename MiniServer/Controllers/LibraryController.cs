using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MiniServer.Models;
using MiniServer.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;

namespace MiniServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private readonly SongerContext _context;
        private readonly int itemsPerPage;
        private readonly ILogger<LibraryController> logger;

        public LibraryController(SongerContext context, IConfiguration configuration, ILogger<LibraryController> logger)
        {
            _context = context;
            itemsPerPage = configuration.GetValue<int>("CustomParameters:itemsPerPage");
            this.logger = logger;
        }

        [HttpGet]
        [Route("Tracks")]
        public JsonResult Tracks(int? from, int? to)
        {
            logger.LogDebug("Retrieving tracks list");
            if (from is null)
                from = 0;
            if (to is null)
                to = from + itemsPerPage;
            else if (to < from)
                to = from;

            IEnumerable<Track> tracks = _context.Tracks
                .OrderBy(track => track.Id)
                .Skip((int)from)
                .Take((int)(to - from))
                .Include(t => t.Album)
                .ThenInclude(a => a.Artist)
                .Select(track => new Track(track));

            logger.LogDebug("Sending {tracksCount} tracks ({requestedTracksCount} requested)", tracks.Count(), to - from);

            return new JsonResult(tracks);
        }

        [HttpGet]
        [Route("Tracks/{id}")]
        public ActionResult<Track> Track(int ID)
        {

            logger.LogInformation("Retrieving track {trackID} info", ID);

            Tracks track = _context.Tracks
                .Include(track => track.Album)
                .ThenInclude(album => album.Artist)
                .FirstOrDefault(track => track.Id == ID);
            if (track is null)
            {
                logger.LogDebug("Track {trackID} info not found", ID);
                return NotFound();
            }
            else
            {
                logger.LogDebug("Sending track {trackID} - {trackTitle} info", track.Id, track.Name);
                return new Track(track);
            }
        }

        [HttpGet]
        [Route("Tracks/{id}/Play")]
        public IActionResult Play(int ID)
        {
            logger.LogInformation("Retrieving track {trackID} stream", ID);

            Tracks track = _context.Tracks
                .Include(track => track.Album)
                .ThenInclude(album => album.Artist)
                .FirstOrDefault(track => track.Id == ID);

            if (track is null)
            {
                logger.LogInformation("Track {trackID} info not found", ID);
                return NotFound();
            }

            logger.LogInformation("Retrieving track {trackID} - {trackTitle} file", track.Id, track.Name);
            FileInfo fileInfo = new FileInfo(track.Filepath);

            if (!fileInfo.Exists)
            {
                logger.LogInformation("Track {trackID} - {trackTitle} not found at {trackFile}", track.Id, track.Name, track.Filepath);
                return NotFound();
            }

            logger.LogInformation("Reading request ranges");
            ICollection<RangeItemHeaderValue> ranges = Request.GetRanges();
            long start, end;
            // The request will be treated as normal request if there is no Range header.
            if (ranges is null || false == ranges.Any())
            {
                logger.LogInformation("No ranges found in request, serving whole file");
                start = 0;
                end = fileInfo.Length;
            }
            else
            {
                logger.LogInformation("Found {rangesCount}", ranges.Count);
                if (ranges.Count > 1)
                {
                    logger.LogInformation("Multiple ranges not allowed");
                    return StatusCode((int)HttpStatusCode.BadRequest);
                }
                RangeItemHeaderValue indices = ranges.FirstOrDefault();

                if (indices.From is null || indices.From >= fileInfo.Length)
                {
                    if (indices.From is null)
                        logger.LogInformation("Invalid null start index");
                    else
                        logger.LogInformation("Start range outside of boundaries (file length: {fileLength}, request start index: {startIndex}", indices.From);
                    return StatusCode((int)HttpStatusCode.RequestedRangeNotSatisfiable);
                }
                start = (long)indices.From;
                end = (long)(indices.To != null && indices.To < fileInfo.Length ? indices.To : fileInfo.Length);
            }

            logger.LogDebug("Reading from {startIndex} to {endIndex} ({bytesCount}/{fileLength bytes)", start, end, end - start, fileInfo.Length);
            // We are now ready to produce partial content.
            byte[] buffer = new byte[end - start];
            Stream fs = fileInfo.OpenRead();
            fs.Position = start;
            int read = fs.Read(buffer, 0, buffer.Length);

            Response.Headers.Add("Accept-Ranges", "bytes");
            Response.StatusCode = (int)HttpStatusCode.PartialContent;
            Response.ContentType = new MediaTypeHeaderValue("multipart/byteranges").ToString();
            Response.Headers.Add("Content-Range", $"{start}-{start + read}/{fileInfo.Length}");
            Response.Body.WriteAsync(buffer);

            Response.CompleteAsync();

            return new EmptyResult();

        }

        [HttpGet]
        [Route("Albums")]
        public JsonResult Albums(int? from, int? to)
        {
            logger.LogDebug("Retrieving albums list");
            if (from is null)
                from = 0;
            if (to is null)
                to = from + itemsPerPage;

            IEnumerable<Album> albums = _context.Albums
                .OrderBy(album => album.Id)
                .Skip((int)from - 1)
                .Take((int)(to - from))
                .Include(album => album.Artist)
                .Select(album => new Album(album));

            logger.LogDebug("Sending {albumsCount} albums ({requestedAlbumsCount} requested)", albums.Count(), to - from);

            return new JsonResult(albums);
        }

        [HttpGet]
        [Route("Albums/{id}")]
        public ActionResult<Album> Album(int ID)
        {

            logger.LogInformation("Retrieving album {albumID} info", ID);

            Albums album = _context.Albums
                .Include(album => album.Tracks)
                .Include(album => album.Artist)
                .FirstOrDefault(album => album.Id == ID);
            if (album is null)
            {
                logger.LogDebug("Album {albumID} info not found", ID);
                return NotFound();
            }
            else
            {
                logger.LogDebug("Sending album {albumID} - {albumTitle} info", album.Id, album.Name);
                return new Album(album);
            }
        }

        [HttpGet]
        [Route("Artists")]
        public JsonResult Artists(int? from, int? to)
        {
            logger.LogDebug("Retrieving artists list");
            if (from is null)
                from = 0;
            if (to is null)
                to = from + itemsPerPage;

            IEnumerable<Artist> artists = _context.Artists
                .OrderBy(artist => artist.Id)
                .Skip((int)from - 1)
                .Take((int)(to - from))
                .Include(artist => artist.Albums)
                .Select(artist => new Artist(artist));

            logger.LogDebug("Sending {artistsCount} artists ({requestedArtistsCount} requested)", artists.Count(), to - from);

            return new JsonResult(artists);
        }

        [HttpGet]
        [Route("Artists/{id}")]
        public ActionResult<Artist> Artist(int ID)
        {

            logger.LogInformation("Retrieving artist {artistID} info", ID);

            Artists artist = _context.Artists
                .Include(artist => artist.Albums)
                .ThenInclude(album => album.Tracks)
                .FirstOrDefault(artist => artist.Id == ID);
            if (artist is null)
            {
                logger.LogDebug("Artist {artistID} info not found", ID);
                return NotFound();
            }
            else
            {
                logger.LogDebug("Sending artist {artistID} - {artistTitle} info", artist.Id, artist.Name);
                return new Artist(artist);
            }
        }
    }
}
