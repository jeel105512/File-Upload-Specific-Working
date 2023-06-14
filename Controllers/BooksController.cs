using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Books.Include(b => b.Library);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Library)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["LibraryId"] = new SelectList(_context.Libraries, "Id", "Name");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LibraryId,Title,Summary,Author,Published")] Book book, IFormFile? Photo)
        {
            if (ModelState.IsValid)
            {
                if(Photo != null && Photo.Length > 0)
                {
                    // Check file type using the first two bytes
                    byte[] fileBytes;
                    using(var memoryStream = new MemoryStream())
                    {
                        await Photo.CopyToAsync(memoryStream);
                        fileBytes = memoryStream.ToArray();
                    }

                    // check the file signature for JPEG and PNG
                    if(!IsJpegFile(fileBytes) && !IsPngFile(fileBytes))
                    {
                        ModelState.AddModelError("Photo", "Only .jpeg or .png files are allowed,");
                        ViewData["LibraryId"] = new SelectList(_context.Libraries, "Id", "Name", book.LibraryId);
                        return View(book);
                    }
                }
                book.Photo = await UploadPhoto(Photo);
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LibraryId"] = new SelectList(_context.Libraries, "Id", "Location", book.LibraryId);
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["LibraryId"] = new SelectList(_context.Libraries, "Id", "Name", book.LibraryId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LibraryId,Title,Summary,Author,Published")] Book book, IFormFile? Photo)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // check if a file is uploaded
                if(Photo != null && Photo.Length > 0)
                {
                    // ---------------------------------------------
                    byte[] fileBytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        await Photo.CopyToAsync(memoryStream);
                        fileBytes = memoryStream.ToArray();
                    }

                    if(!IsJpegFile(fileBytes) && !IsPngFile(fileBytes))
                    {
                        ModelState.AddModelError("Photo", "only .jpeg or .png siles are allowed");
                        ViewData["LibraryId"] = new SelectList(_context.Libraries, "Id", "Location", book.LibraryId);
                        return View(book);
                    }
                    // ---------------------------------------------

                    // Delete the old image file
                    if (!string.IsNullOrEmpty(book.Photo))
                    {
                        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "books", book.Photo);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    // Upload the new photo and get the file name
                    string newFileName = await UploadPhoto(Photo);

                    // Update the book's PhotoFileName property with the new file name
                    book.Photo = newFileName;
                }

                try
                {
                    //book.Photo = await UploadPhoto(Photo);
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["LibraryId"] = new SelectList(_context.Libraries, "Id", "Location", book.LibraryId);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Library)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Books'  is null.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            //if (book != null)
            //{
            //    _context.Books.Remove(book);
            //}

            // Delete the associated image file
            if (!string.IsNullOrEmpty(book.Photo)){
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "books", book.Photo);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
          return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // Helper methods
        private async Task<string> UploadPhoto(IFormFile Photo)
        {
            if (Photo == null) return null;

            var filePath = Path.GetTempFileName();
            var fileName = Guid.NewGuid() + "-" + Photo.FileName;
            var uploadPAth = System.IO.Directory.GetCurrentDirectory() + "\\wwwroot\\images\\books\\" + fileName;

            using var stream = new FileStream(uploadPAth, FileMode.Create);
            await Photo.CopyToAsync(stream);

            return fileName;
        }

        // Helper method to check if the file is a JPEG (JFIF)
        private bool IsJpegFile(byte[] fileBytes)
        {
            return fileBytes.Length > 2 && fileBytes[0] == 0xFF && fileBytes[1] == 0xD8;
        }

        // Helper method to check if the file is a PNG
        private bool IsPngFile(byte[] fileBytes)
        {
            return fileBytes.Length > 8 &&
                fileBytes[0] == 0x89 &&
                fileBytes[1] == 0x50 &&
                fileBytes[2] == 0x4E &&
                fileBytes[3] == 0x47 &&
                fileBytes[4] == 0x0D &&
                fileBytes[5] == 0x0A &&
                fileBytes[6] == 0x1A &&
                fileBytes[7] == 0x0A;
        }
    }
}
