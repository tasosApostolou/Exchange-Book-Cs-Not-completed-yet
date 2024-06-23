﻿using AutoMapper;
using ExchangeBook.Data;
using ExchangeBook.DTO.AuthorDTOs;
using ExchangeBook.DTO.BookDTOs;
using ExchangeBook.Services;
using ExchangeBook.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ExchangeBook.Controllers
{
    public class PersonController :BaseController
    {
        private readonly IMapper _mapper;

        public PersonController(IApplicationService applicationService,
        IMapper mapper) : base(applicationService)
        {
            _mapper = mapper;
        }

        [HttpPost("{personId}/books")]
        public async Task<IActionResult> AddBookToPerson(int personId,[FromBody] BookInsertDTO bookDto)
        {
            if (bookDto == null)
            {
                return NotFound();
            }
            var book = await _applicationService.BookService.CreateBookAsync(bookDto);

            // Add the created book to the person's books (assigning personId and Bookid in Person_Book many-many table
            await _applicationService.PersonService.AddBookToPersonAsync(personId, book.Id);

            return Ok(_mapper.Map<BookReadOnlyDTO>(book));
            //return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<BookReadOnlyDTO>> GetBook(int id)
        {
            Book? book = await _applicationService.BookService.GetBookWithAuthorById(id);
            if (book is null)
            {
                throw new BookNotFoundException("Book NotFound");
            }

            BookReadOnlyDTO returnedBook = _mapper.Map<BookReadOnlyDTO>(book);
            //AuthorReadonlyDTO authorReadonly= _mapper.Map<AuthorReadonlyDTO>(book.Author);
           

            //returnedBook.Author= authorReadonly;
            //returnedBook.Author = _mapper.Map<AuthorReadonlyDTO>(book.Author);
            //Console.WriteLine("malakies " + authorReadonly.Name);
            return Ok(returnedBook);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Book>>> GetPersonBooks(int? id)
        {
            List<Book> books = await _applicationService.PersonService.GetPersonBooksAsync(id);
            List<BookReadOnlyDTO> booksReadOnly;
            if(books is null)
            {
                throw new BookNotFoundException("not found books");
            }         
                return Ok(_mapper.Map<List<BookReadOnlyDTO>>(books));        
        }

        [HttpDelete("personal/{personId}/book/{bookId}")]
        public async Task<IActionResult> DeletesPersonBook(int personId, int bookId)
        {
          
             bool deleted = await _applicationService.PersonService.DeletePersonBookAsync(personId, bookId);
            if (!deleted)
            {
                throw new ServerGenericException("book not found or not deleted or server erro");
            }
                return Ok("Deleted");
        }
    }
}