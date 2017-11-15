using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persona
{
    [Serializable]
    class Person{
        public String name;
        public String lastname;
        public String patronymic;
        public String phone;
        public String seria;
        public String num;
        public String id_road;

        public String vvod(String info){
            Console.WriteLine(info);
            String data = "";
            while (data == ""){
                data = Console.ReadLine();
            }
            return data;
        }


    }
}
