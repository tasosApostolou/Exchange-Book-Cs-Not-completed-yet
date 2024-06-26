﻿using ExchangeBook.Data;

namespace ExchangeBook.Repositories
{
    public interface IStoreBookRepository
    {
        Task<List<StoreBook>> GetByBookTitleAsync(string? title);
        Task<List<StoreBook>> GetStoreBooksByStoreIdAsync(int storeId);
        //Task<bool> RemoveBookFromStoreAsync(int storeId, int bookId);
        Task<StoreBook> RemoveBookFromStoreAsync(int storeId, int bookId);
    }
}
