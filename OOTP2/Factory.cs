using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOTP2
{
    public abstract class PersonFactory
    {
        public abstract Person CreatePerson();
    }

    public class UserFactory : PersonFactory
    {
        public override Person CreatePerson()
        {
            return new User();
        }
    }

    public class AuthorFactory : PersonFactory
    {
        public override Person CreatePerson()
        {
            return new Author();
        }
    }

    public class WorkerFactory : PersonFactory
    {
        public override Person CreatePerson()
        {
            return new Worker();
        }
    }

    public class SecurityFactory : PersonFactory
    {
        public override Person CreatePerson()
        {
            return new Security();
        }
    }

    public abstract class ObjectFactory<T>
    {
        public abstract T CreateObject();
    }

    public class BookFactory : ObjectFactory<Book>
    {
        public override Book CreateObject()
        {
            return new Book();
        }
    }

}
