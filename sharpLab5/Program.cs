using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using Persona;
using System.Runtime.Serialization.Formatters.Binary;
//добавить делегаты и виртуальные функции
namespace sharpLab5
{
    class routes
    {
        public String id;
        public String name;
        public String date;
        public String time;
        public int free;
        public int occupied;

        public void set(XElement el)
        {
            id = el.Element("id").Value;
            name = el.Element("name").Value;
            date = el.Element("date").Value;
            time = el.Element("time").Value;
            free = Convert.ToInt16(el.Element("free").Value);
            occupied = Convert.ToInt16(el.Element("occupied").Value);
        }
    }

    class Program
    {
        static String classpath = @"C:\Users\user\Documents\Visual Studio 2015\Projects\sharpLab5\lab5.xml";
        static String textpath = @"C:\Users\user\Documents\Visual Studio 2015\Projects\sharpLab5\lab5.txt";

        static XElement createRoute(String id, String name, String date, String time, String free)
        {
            XElement road = new XElement("road",
                        new XElement("id", id),
                        new XElement("name", name),
                        new XElement("date", date),
                        new XElement("time", time),
                        new XElement("free", free),
                        new XElement("occupied", "0")
                    );
            return road;
        }
        public static void Serialize(List<Person> PersonList)
        {
            FileStream fstr = new FileStream(textpath, FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            if (PersonList.Count > 0)
            {
                int i = 0;
                while (i < PersonList.Count)
                {
                    bf.Serialize(fstr, PersonList.ElementAt(i));
                    i++;
                }
            }
            fstr.Close();
        }

        static Person person()
        {
            Person p = new User();
            p.name = p.vvod("Введите имя:");
            p.lastname = p.vvod("Введите фамилию:");
            p.patronymic = p.vvod("Введите отчество:");
            p.phone = p.vvod("Введите номер телефона:");
            p.seria = p.vvod("Введите серию паспорта:");
            p.num = p.vvod("Введите номер паспорта:");

            return p;
        }


        static void Main(string[] args)
        {
            List<routes> list = new List<routes>();
            XElement root;

            if (!File.Exists(classpath))
            {
                root = new XElement("Buses",
                    createRoute("0", "A-B", "01.10.10", "01:25", "30"),
                    createRoute("1", "B-C", "02.10.10", "11:25", "40"),
                    createRoute("2", "C-D", "03.10.10", "12:55", "50")
                );
                root.Save(classpath);
            }
            root = XElement.Load(classpath);
            foreach (XElement el in root.Elements("road"))
            {
                routes route = new routes();
                route.set(el);
                list.Add(route);
            }

            List<Person> PersonList = new List<Person>();

            if (File.Exists(textpath))
            {
                FileStream fstr = new FileStream(textpath, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(fstr);
                reader.ReadToEnd();
                if (fstr.Position != 0)
                {
                    {
                        long length = fstr.Position;
                        fstr.Seek(0, SeekOrigin.Begin);
                        BinaryFormatter bf = new BinaryFormatter();
                        while (fstr.Position < length)
                        {
                            Person pr = (Person)bf.Deserialize(fstr);
                            PersonList.Add(pr);
                        }
                    }
                    reader.Close();
                }
                fstr.Close();
            }

            String choise = null;
            while (choise != "exit")
            {
                Console.WriteLine("1) Приобрести билет");
                Console.WriteLine("2) Вернуть билет");
                choise = Console.ReadLine();
                Console.Clear();
                switch (choise)
                {
                    case "1":
                        {
                            do
                            {
                                int count = 0;
                                try
                                {
                                    Console.WriteLine("Выберите маршрут:");
                                    foreach (routes el in list)
                                    {
                                        Console.WriteLine(count + 1 + ") " + el.name + "date: " + el.date);
                                        count++;
                                    }
                                    choise = Console.ReadLine();
                                    int num = Convert.ToInt16(choise);
                                    Console.Clear();
                                    if (num > 0 && num <= list.Count)
                                    {
                                        Console.WriteLine("Информация о маршруте:");
                                        num--;
                                        routes rt = list.ElementAt(num);
                                        Console.WriteLine(rt.name + "\n" + rt.date + "\n" + rt.time);
                                        Console.WriteLine("Свободное количество мест:" + rt.free);
                                        Console.WriteLine("Мест занято:" + rt.occupied);
                                        Console.WriteLine("Приобрести билет? Y(да)/N(нет)");
                                        String str = Console.ReadLine();
                                        if (rt.free < 1)
                                        {
                                            Console.WriteLine("Свободных мест нет!");
                                            continue;
                                        }
                                        if (str.ToLower().Equals("y"))
                                        {
                                            Person prsn = person();
                                            prsn.id_road = rt.id;
                                            FileStream fstr = new FileStream(textpath, FileMode.OpenOrCreate, FileAccess.Write);
                                            fstr.Seek(0, SeekOrigin.End);
                                            BinaryFormatter bf = new BinaryFormatter();
                                            bf.Serialize(fstr, prsn);
                                            fstr.Close();
                                            PersonList.Add(prsn);
                                            Console.WriteLine("Билет приобретен!");
                                            rt.occupied++;
                                            rt.free--;
                                        }
                                        else Console.WriteLine("Билет не приобретен!");

                                    }
                                    else
                                    {
                                        Console.WriteLine("Проверьте правильность ввода!");
                                    }


                                    root = new XElement("Buses");
                                    foreach (routes el in list)
                                    {
                                        XElement road = new XElement("road",
                                            new XElement("id", el.id),
                                            new XElement("name", el.name),
                                            new XElement("date", el.date),
                                            new XElement("time", el.time),
                                            new XElement("free", el.free),
                                            new XElement("occupied", el.occupied)
                                        );
                                        root.Add(road);

                                    }
                                    root.Save(classpath);

                                }
                                catch (Exception ex)
                                {
                                    if (choise.Equals("back"))
                                        Console.WriteLine("Выход...");
                                    else
                                        Console.WriteLine("Ошибка: " + ex.Message);
                                }
                                Console.ReadLine();
                                Console.Clear();
                            } while (choise != "back");

                            break;
                        }
                    case "2":
                        {
                            Console.WriteLine("Введите номер телефона для возврата билета:");
                            String phone = Console.ReadLine();
                            if (phone.Length > 0)
                            {

                                var data = from d in PersonList
                                           where d.phone.Equals(phone)
                                           select d;

                                foreach (Person prsn in data)
                                {
                                    if (prsn.phone.Equals(phone))
                                    {

                                        root = new XElement("Buses");

                                        foreach (routes road in list)
                                        {
                                            if (prsn.id_road == road.id)
                                            {
                                                road.free++;
                                                road.occupied--;
                                            }
                                            root.Add(createRoute(road.id,road.name,road.date,road.time, Convert.ToString(road.free)));
                                        }
                                        root.Save(classpath);
                                        PersonList.Remove(prsn);
                                        Serialize(PersonList);

                                      
                                        Console.WriteLine("Билет возвращен");
                                        break;
                                    }
                                }
                            }
                            else Console.WriteLine("Проверьте правильность ввода");
                            break;
                        }
                    default: break;
                }
                Console.Clear();
            }

        }
    }
    
}
