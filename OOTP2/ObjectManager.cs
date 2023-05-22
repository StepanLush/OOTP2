using OOTP2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOTP2
{
    public static class Managers
    {
        public static BookManager BookManager { get; } = new BookManager();
        public static PersonManager PersonManager { get; } = new PersonManager();
        public static UserManager UserManager { get; } = new UserManager();
        public static AuthorManager AuthorManager { get; } = new AuthorManager();
        public static WorkerManager WorkerManager { get; } = new WorkerManager();
        public static SecurityManager SecurityManager { get; } = new SecurityManager();
    }
    public class BookManager
    {
        private List<Book> books;

        public BookManager()
        {
            books = new List<Book>();
        }

        public void AddBook(Book book)
        {
            books.Add(book);
        }

        public List<Book> GetBooks()
        {
            return books;
        }

        public void UpdateBook(Book updatedBook)
        {
            Book existingBook = books.Find(b => b.Name == updatedBook.Name);
            if (existingBook != null)
            {
                existingBook.Author = updatedBook.Author;
                existingBook.Genre = updatedBook.Genre;
                existingBook.Place = updatedBook.Place;
            }
        }

        public void DeleteBook(Book book)
        {
            books.Remove(book);
        }
    }

    public class PersonManager
    {
        private List<Person> persons;

        public PersonManager()
        {
            persons = new List<Person>();
        }

        public void AddPerson(Person person)
        {
            persons.Add(person);
        }

        public List<Person> GetPersons()
        {
            return persons;
        }

        public void UpdatePerson(Person updatedPerson)
        {
            Person existingPerson = persons.Find(p => p.Name == updatedPerson.Name);
            if (existingPerson != null)
            {
                existingPerson.Email = updatedPerson.Email;
                existingPerson.Age = updatedPerson.Age;
            }
        }

        public void DeletePerson(Person person)
        {
            persons.Remove(person);
        }
    }

    public class UserManager
    {
        private List<User> users;

        public UserManager()
        {
            users = new List<User>();
        }

        public void AddUser(User user)
        {
            users.Add(user);
        }

        public List<User> GetUsers()
        {
            return users;
        }

        public void UpdateUser(User updatedUser)
        {
            User existingUser = users.Find(u => u.Name == updatedUser.Name);
            if (existingUser != null)
            {
                existingUser.Email = updatedUser.Email;
                existingUser.Age = updatedUser.Age;
                existingUser.IsDept = updatedUser.IsDept;
                existingUser.BookList = updatedUser.BookList;
            }
        }

        public void DeleteUser(User user)
        {
            users.Remove(user);
        }
    }

    public class AuthorManager
    {
        private List<Author> authors;

        public AuthorManager()
        {
            authors = new List<Author>();
        }

        public void AddAuthor(Author author)
        {
            authors.Add(author);
        }

        public List<Author> GetAuthors()
        {
            return authors;
        }

        public void UpdateAuthor(Author updatedAuthor)
        {
            Author existingAuthor = authors.Find(a => a.Name == updatedAuthor.Name);
            if (existingAuthor != null)
            {
                existingAuthor.Email = updatedAuthor.Email;
                existingAuthor.Age = updatedAuthor.Age;
                existingAuthor.IsAlive = updatedAuthor.IsAlive;
            }
        }

        public void DeleteAuthor(Author author)
        {
            authors.Remove(author);
        }
    }

    public class WorkerManager
    {
        private List<Worker> workers;

        public WorkerManager()
        {
            workers = new List<Worker>();
        }

        public void AddWorker(Worker worker)
        {
            workers.Add(worker);
        }

        public List<Worker> GetWorkers()
        {
            return workers;
        }

        public void UpdateWorker(Worker updatedWorker)
        {
            Worker existingWorker = workers.Find(w => w.Name == updatedWorker.Name);
            if (existingWorker != null)
            {
                existingWorker.Email = updatedWorker.Email;
                existingWorker.Age = updatedWorker.Age;
                existingWorker.Salary = updatedWorker.Salary;
                existingWorker.Place = updatedWorker.Place;
            }
        }
        public void DeleteWorker(Worker worker)
        {
            workers.Remove(worker);
        }
    }
    public class SecurityManager
    {
        private List<Security> securities;
        public SecurityManager()
        {
            securities = new List<Security>();
        }

        public void AddSecurity(Security security)
        {
            securities.Add(security);
        }

        public List<Security> GetSecurities()
        {
            return securities;
        }

        public void UpdateSecurity(Security updatedSecurity)
        {
            Security existingSecurity = securities.Find(s => s.Name == updatedSecurity.Name);
            if (existingSecurity != null)
            {
                existingSecurity.Email = updatedSecurity.Email;
                existingSecurity.Age = updatedSecurity.Age;
                existingSecurity.Salary = updatedSecurity.Salary;
                existingSecurity.Place = updatedSecurity.Place;
                existingSecurity.Shift = updatedSecurity.Shift;
            }
        }

        public void DeleteSecurity(Security security)
        {
            securities.Remove(security);
        }
    }

}