

using DotnetAPI.Models;

namespace DotnetAPI.Data 

    {
        public class UserRepository: IUserRepository  {
                DataContextEF _entityFramework;
  
         public UserRepository(IConfiguration config ) {

            _entityFramework = new DataContextEF(config);


         }

        public bool SaveChanges() {
            return _entityFramework.SaveChanges() > 0;
        }

        public void AddEntity<T>(T entityToAdd) {
            if(entityToAdd !=null){
                _entityFramework.Add(entityToAdd);
            }
           
        }

        public void RemoveEntity<T>(T entityToRemove){
            if(entityToRemove !=null){
                _entityFramework.Remove(entityToRemove);
            }
        }
        

     public IEnumerable<User> getUsers()

        {
            
            IEnumerable<User> response = _entityFramework.Users.ToList<User>();
            

            return response;
        }
     
      public User getSingleUser(int userId)

        {
            
            User? user = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault<User>();

            if(user != null){
               return user;
            }

            throw new Exception("User not found");
        }
     


     
        }
    }