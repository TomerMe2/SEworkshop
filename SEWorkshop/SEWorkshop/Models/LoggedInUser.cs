using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    public class LoggedInUser : User
    {
        public IList<Store> Owns { get; private set; }
        public IList<Store> Manages { get; private set; }
        public IList<Review> Reviews { get; private set; }
        public IList<Message> Messages { get; private set; }
        public IList<Purchase> Purchase_Histroy{get; private set;}
        public string Username {get; private set;}
        public string Password {get; private set;} 

        public LoggedInUser(string username, string password)
        {
            Username=username;
            Password=password;
            Owns = new List<Store>();
            Manages = new List<Store>();
            Reviews = new List<Review>();
            Messages = new List<Message>();
            Purchase_Histroy= new List<Purchase>();
        }

        public void Logout(){
            Facades.LoggedInUserFacade.getInstance().Logout();
        }

        public void OpenStore(string store_name){
            Store new_store= new Store(Facades.LoggedInUserFacade.getInstance().LoggedInUser, store_name);
            Owns.Add(new_store);
            Console.WriteLine("store: "+ store_name + " has opened\n");

        } 

        public Boolean PurchaseHistoryContainsProduct(Product pro){
            foreach(Purchase p in Purchase_Histroy){
                foreach(Cart c in p){
                    foreach(Product prod in c){
                        if(prod.Name ==pro.Name && pro.Store==prod.Store){
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void WriteReview(string review, Product p){
            if (PurchaseHistoryContainsProduct(p) && review != ""){
                Review new_review= new Review(review, p);
                Reviews.Add(this.RegisteredUser ,p,review);
                 Console.WriteLine("Review has been sent\n");
            }
            else {
                Console.WriteLine("Unable to send review\n");
            }

        }

        public void SendMessage(string message, Store store){
            if(message!=""){

                Messages new_message= new Message(store,this.RegisteredUser,Messages.Last() ,null);
                if(Messages.Last() != null){
                    Messages.Last().Next()= new_message;
                }
                Messages.Add(new_message);
                 Console.WriteLine("Message has been sent\n");
            }
            else{
                 Console.WriteLine("Unable to send message\n");
            }
        }

        public void ViewPurchaseHistory(){
            foreach(Purchase p in Purchase_Histroy){
               foreach(Cart c in p){
                   foreach(Basket b in c){
                       foreach(Product pr in b){
                        Console.WriteLine("Product name: "+ pr.product_name+ " Store: "+ pr.Store+ " Policy: "+ pr.Policy +"\n");
                       }
                   }
               }
            }
        }
    }
}
