using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Xml.Serialization;
using System.Collections;
using System.Security.Policy;
using System.Runtime.InteropServices;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace BasketballData
{
    class Program
    {
        public class Node
        {
            public string last, first, age, min, fieldGoal, fgAttempts;

            public Node next;

            public Node(string tlast, string tfirst, string tage, string tmin, string tfieldGoal, string tfgAttempts)
            {
                last = tlast;
                first = tfirst;
                age = tage;
                min = tmin;
                fieldGoal = tfieldGoal;
                fgAttempts = tfgAttempts;
                next = null;
            }
        }

        public class LinkList
        {
            public Node head;
            public int count;
            public LinkList()
            {
                head = null;
            }

            public void addend(string tlast, string tfirst, string tage, string tmin, string tfieldGoal, string tfgAttempts)
            {
                Node temp = new Node(tlast, tfirst, tage, tmin, tfieldGoal, tfgAttempts);
                if (head == null)
                {
                    head = temp;
                    count++;
                }
                else
                {
                    Node cu = head;
                    while (cu.next != null)
                    {
                        cu = cu.next;
                    }
                    cu.next = temp;
                    count++;

                }
            }

            public void addbeg(string tlast, string tfirst, string tage, string tmin, string tfieldGoal, string tfgAttempts)
            {
                if (head == null)
                {
                    Node temp = new Node(tlast, tfirst, tage, tmin, tfieldGoal, tfgAttempts);
                    head = temp;
                    count++;
                }

                else
                {
                    Node temp = new Node(tlast, tfirst, tage, tmin, tfieldGoal, tfgAttempts);
                    temp.next = head;
                    head = temp;
                    count++;
                }
            }

            public void search(string name)
            {
                int index = 1;
                Node cu = head;
                while (cu.next != null)
                {
                    if (cu.last == name)
                    {
                        break;
                    }
                    else
                    {
                        cu = cu.next;
                    }
                    index++;
                }
                Console.WriteLine("Search complete! Index: " + index + "\n" + cu.last + ", " + cu.first + ", " + cu.age + ", " + cu.min + ", " + cu.fieldGoal + ", " + cu.fgAttempts);
            }

            public void delete(string name)
            {
                if (head == null)
                {
                    Console.WriteLine("Empty List!!!!!!!!");
                    return;
                }
                else
                {
                    Node cu = head;
                    Node prev = null;

                    while (cu != null)
                    {
                        if (cu.last == name)
                        {
                            break;
                        }
                        else
                        {
                            prev = cu;
                            cu = cu.next;
                        }
                        if (cu == null)
                        {
                            Console.WriteLine("Couldnt find player");
                        }
                        else
                        {
                            if (head == cu)
                            {
                                head = head.next;
                            }
                            else
                            {
                                prev.next = cu.next;
                            }
                        }
                    }
                }
            }

            public void print(Node head)
            {
                if (head == null)
                {
                    return;
                }

                else
                {
                    Console.WriteLine(head.last + ", " + head.first + ", " + head.age + ", " + head.min + ", " + head.fieldGoal + ", " + head.fgAttempts);
                    print(head.next);
                }
            }

            public void readtxt(string file)
            {

                using (StreamReader sr = File.OpenText(file))
                {
                    string s = String.Empty;
                    while ((s = sr.ReadLine()) != null)
                    {
                        s = s.Replace(" ", String.Empty);
                        //delimit by comma
                        string[] tokens = s.Split(',');
                        //add to linked list
                        addend(tokens[0], tokens[1], tokens[2], tokens[3], tokens[4], tokens[5]);
                    }
                }
                print(head);
                Console.WriteLine("\n\n");

            }

            SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-7P99TDD\MYMSSQLSERVER;Initial Catalog=Project1;Integrated Security=True;");
            SqlCommand cmd;
            public void sql()
            {
                //reads file and adds it to linked list

                con.Open();
                Node cu = head;
                while (cu.next != null)
                {
                    //Inserts values into SQL
                    cmd = new SqlCommand("insert into Basketball values('" + cu.last + "', '" + cu.first + "', '" + cu.age + "', '" + cu.min + "', '" + cu.fieldGoal + "', '" + cu.fgAttempts + "')", con);
                    cmd.ExecuteNonQuery();
                    cu = cu.next;
                }

                con.Close();
            }

        }

        public struct MenuChoice
        {
            public string last, first, age, min, fieldGoal, fgAttempts;
        };

        public static MenuChoice casefunc()
        {
            MenuChoice m;
            Console.WriteLine("Please enter values in order: Last Name, First Name, Age, Minutes, Field Goals, Field Goal Attempts");
            m.last = Console.ReadLine();
            m.first = Console.ReadLine();
            m.age = Console.ReadLine();
            m.min = Console.ReadLine();
            m.fieldGoal = Console.ReadLine();
            m.fgAttempts = Console.ReadLine();

            return m;
        }  /// makes the main easier to read

        public static int menu()
        {

            Console.WriteLine("1. Add Beginning");
            Console.WriteLine("2. Add Middle");
            Console.WriteLine("3. Add End");
            Console.WriteLine("4. Search Node");
            Console.WriteLine("5. Delete Node");
            Console.WriteLine("6. Quit");
            Console.Write("Choose an option: ");
            int choice = Convert.ToInt32(Console.ReadLine());
            return choice;
        }

        public class MongoCRUD
        {
            IMongoDatabase db;

            public MongoCRUD(string database)
            {
                var client = new MongoClient();
                db = client.GetDatabase(database);

            }

            public void InsertRecord<T>(string table, T record)
            {
                var collection = db.GetCollection<T>(table);
                collection.InsertOne(record);
            }
        }
        public class PlayerModel
        {
            [BsonId]
            public Guid Id { get; set; }
            public string last { get; set; }
            public string first { get; set; }
            public string age { get; set; }
            public string min { get; set; }
            public string fg { get; set; }
            public string fgA { get; set; }

        }
        static void Main(string[] args)
        {
            LinkList l = new LinkList();
            // reads file and adds it to linked list
            l.readtxt("inputdata.txt");


            //////MENU

            int choice = 0;
            MenuChoice m;
            while (choice != 6)
            {
                choice = menu();
                switch (choice)
                {
                    case 1:  //add beginning
                        m = casefunc();
                        l.addbeg(m.last, m.first, m.age, m.min, m.fieldGoal, m.fgAttempts);
                        l.print(l.head);
                        Console.WriteLine("\n");
                        break;
                    case 2: // addmiddle TODO
                        m = casefunc();
                        //l.addbeg(last, first, age, min, fieldGoal, fgAttempts);
                        l.print(l.head);
                        Console.WriteLine("\n");
                        break;
                    case 3:  // add end
                        m = casefunc();
                        l.addend(m.last, m.first, m.age, m.min, m.fieldGoal, m.fgAttempts);
                        l.print(l.head);
                        Console.WriteLine("\n");
                        break;
                    case 4:  // search by last name
                        Console.Write("Choose last name to search: ");
                        string lname = Console.ReadLine();
                        l.search(lname);
                        Console.WriteLine("\n");
                        break;
                    case 5:  // delete, doesnt fully work yet
                        Console.Write("Choose last name to delete: ");
                        lname = Console.ReadLine();
                        l.delete(lname);
                        l.print(l.head);
                        Console.WriteLine("\n");
                        break;
                    case 6:
                        break;
                }
            }

            Console.WriteLine("Do you want to add the data to the SQL database? Y/N");

            string decision = Console.ReadLine();
            if (decision == "Y")
            {
                //adds to SQL database
                //l.sql();
                Node cu = l.head;
                while (cu.next != null)
                {
                    MongoCRUD db = new MongoCRUD("BasketballStats");
                    db.InsertRecord("Players", new PlayerModel
                    {
                        last = cu.last,
                        first = cu.first,
                        age = cu.age,
                        min = cu.min,
                        fg = cu.fieldGoal,
                        fgA = cu.fgAttempts
                    });
                    cu = cu.next;
                }
            }
            else
            {
                return;
            }






            Console.ReadLine();
        }
    }
}
