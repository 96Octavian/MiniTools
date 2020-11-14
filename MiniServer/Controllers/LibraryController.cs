using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MiniServer.Models;
using MiniServer.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MiniServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private readonly SongerContext _context;
        private readonly int itemsPerPage;

        public LibraryController(SongerContext context, IConfiguration configuration)
        {
            _context = context;
            itemsPerPage = configuration.GetValue<int>("CustomParameters:itemsPerPage");
        }

        [HttpGet]
        [Route("Tracks")]
        public JsonResult Tracks(int? from, int? to)
        {
            if (from is null)
                from = 0;
            if (to is null)
                to = from + itemsPerPage;

            IEnumerable<Track> tracks = _context.Tracks
                .OrderBy(track => track.Id)
                .Skip((int)from - 1)
                .Take((int)(to - from))
                .Include(t => t.Album)
                .ThenInclude(a => a.Artist)
                .Select(track => new Track(track));

            return new JsonResult(tracks);
        }

        [HttpGet]
        [Route("Tracks/{id}")]
        public ActionResult<Track> Track(int ID)
        {
            Tracks track = _context.Tracks
                .Include(track => track.Album)
                .ThenInclude(album => album.Artist)
                .FirstOrDefault(track => track.Id == ID);
            if (track is null)
                return NotFound();
            else
                return new Track(track);
        }

        [HttpGet]
        [Route("Tracks/{id}/Play")]
        public IActionResult Play(int ID)
        {

            Tracks track = _context.Tracks
                .Include(track => track.Album)
                .ThenInclude(album => album.Artist)
                .FirstOrDefault(track => track.Id == ID);

            if (track is null)
                return NotFound();

            FileInfo fileInfo = new FileInfo(track.Filepath);

            if (!fileInfo.Exists)
                return NotFound();

            ICollection<RangeItemHeaderValue> ranges = Request.GetRanges();

            long start, end;
            // The request will be treated as normal request if there is no Range header.
            if (ranges is null || false == ranges.Any())
            {
                start = 0;
                end = fileInfo.Length;
            }
            else
            {

                if (ranges.Count > 1)
                    return StatusCode((int)HttpStatusCode.BadRequest);

                RangeItemHeaderValue indices = ranges.FirstOrDefault();

                if (indices.From is null || indices.From >= fileInfo.Length)
                    return StatusCode((int)HttpStatusCode.RequestedRangeNotSatisfiable);

                start = (long)indices.From;
                end = (long)(indices.To != null && indices.To < fileInfo.Length ? indices.To : fileInfo.Length);
            }

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

            return new JsonResult(albums);
        }

        [HttpGet]
        [Route("Artists")]
        public JsonResult Artists(int? from, int? to)
        {
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

            return new JsonResult(artists);
        }
    }
}
