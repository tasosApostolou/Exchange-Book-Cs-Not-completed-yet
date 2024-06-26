﻿using AutoMapper;
using ExchangeBook.Data;
using ExchangeBook.DTO.BookDTOs;
using ExchangeBook.DTO.PersonDTO;
using ExchangeBook.DTO.UserDTOs;
using ExchangeBook.Repositories;
using ExchangeBook.Services.Exceptions;

namespace ExchangeBook.Services
{
    public class PersonService : IPersonService
    {
        private readonly IUnitOfWork? _unitOfWork;
        private readonly ILogger<UserService>? _logger;
        private readonly IMapper? _mapper;

        public PersonService(IUnitOfWork? unitOfWork, ILogger<UserService>? logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task AddBookToPersonAsync(int personId, int bookId)
        {
            Book? book;
            Person? person;
            try
            {
                person = await _unitOfWork.PersonRepository.GetAsync(personId);
                if (person == null)
                {
                    throw new Exception();
                }
                book = await _unitOfWork.BookRepository.GetAsync(bookId);
                if (book == null)
                {
                    throw new Exception("Book not found.");
                }
                //person.Books.Add(book);
                await _unitOfWork.PersonRepository.AddBookToPersonAsync(person, book);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
                throw;
            }
        }

        public async Task<List<Book>> GetPersonBooksAsync(int? id)
        {
            List<Book> books = new();

            try
            {
                books = await _unitOfWork!.PersonRepository.GetPersonBooksAsync(id);
                _logger!.LogInformation("{Message}", "Student count retrieved with success");
            }
            catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
            }
            return books;
        }

        public async Task<BookReadOnlyDTO?> DeletePersonBookAsync(int personId, int bookId)
        {
            bool deleted = false;
            Person person;
            Book? book;
            BookReadOnlyDTO? bookToReturn;
            try
            {
                if (await _unitOfWork.PersonRepository.GetAsync(personId) == null) throw new PersonNotFoundException("Person not found");
                book = await _unitOfWork.BookRepository.GetAsync(bookId);
                if (book == null) throw new BookNotFoundException("Book Not Found");
                bookToReturn = _mapper.Map<BookReadOnlyDTO>(book);
                deleted = await _unitOfWork.PersonRepository.DeletePersonBookAsync(personId, bookId);
                if (!deleted)
                {
                    throw new ServerGenericException("Book Not deleted");
                }
                return bookToReturn;
            }
            catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
                return null;
            }
           
        }

        public async Task<Person?> UpdatePersonAsync(int personId, PersonDTO personDTO)
        {
            Person? existingPerson;
            Person? person;
            var personUser = await _unitOfWork.UserRepositorty.GetAsync(personDTO.UserId);
            if (personUser == null) return null;
            try
            {
                var personToUpdate = _mapper!.Map<Person>(personDTO);

                person = await _unitOfWork.PersonRepository.UpdatePersonAsync(personId, personToUpdate);
                if (person is null) return null;
                person.User = personUser;
                await _unitOfWork.SaveAsync();
                _logger!.LogInformation("{Message}", "User updated successfully");
            }
            catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
                throw;
            }
            return person;
        }

        public async Task DeletePersonAsync(int id)
        {
            Person person;
            try
            {
                person = await _unitOfWork!.PersonRepository.GetAsync(id);
                if (person is null)
                {
                    throw new PersonNotFoundException("Person not found");
                }
                Person? deletedPerson = await _unitOfWork!.PersonRepository.DeleteAsync(id);
                if (deletedPerson != null)
                {
                    throw new UserNotFoundException("Person Not Deleted");
                }
                await _unitOfWork.SaveAsync(); // Ensure changes are saved to the database

            }
            catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
                throw;
            }
        }
    }
}
