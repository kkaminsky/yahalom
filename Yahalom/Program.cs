using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Yahalom
{
    class UserRSA
    {
        public int id;
        public BigInteger n;
        private BigInteger p;
        private BigInteger q;
        public BigInteger d;
        public BigInteger e;
        public BigInteger k;

        public UserRSA()
        {
            id = new Random().Next(0, 100);
            p = BigInteger.ProbablePrime(1024, new Random());
            q = BigInteger.ProbablePrime(1024, new Random());
            n = p.Multiply(q);
            k = n;

            e = n; 
            
            BigInteger q_1 = q.Subtract(BigInteger.One);
            BigInteger p_1 = p.Subtract(BigInteger.One);
            while (e.CompareTo(q_1.Multiply(p_1)) != -1 && e.Gcd(q_1.Multiply(p_1)) != BigInteger.One && e == n)
            {
                e = BigInteger.ProbablePrime(1024, new Random());
            }
            while (k.CompareTo(q_1.Multiply(p_1)) != -1 && k.Gcd(q_1.Multiply(p_1)) != BigInteger.One)
            {
                k = BigInteger.ProbablePrime(1024, new Random());
            }

            d = e.ModInverse(q_1.Multiply(p_1));
            
        }
        public UserRSA(int BitLenght)
        {
            id = new Random().Next(0, 100);
            p = BigInteger.ProbablePrime(BitLenght, new Random());
            q = BigInteger.ProbablePrime(BitLenght, new Random());
            n = p.Multiply(q);
            e = n;
            k = n;

            BigInteger q_1 = q.Subtract(BigInteger.One);
            BigInteger p_1 = p.Subtract(BigInteger.One);
            while (e.CompareTo(q_1.Multiply(p_1)) != -1 && e.Gcd(q_1.Multiply(p_1)) != BigInteger.One && e==n)
            {
                e = BigInteger.ProbablePrime(BitLenght, new Random());
            }
            A:  try
            {
                while (k.CompareTo(q_1.Multiply(p_1)) != -1 && k.Gcd(q_1.Multiply(p_1)) != BigInteger.One)
                {
                    k = BigInteger.ProbablePrime(BitLenght, new Random());
                }
                var t = BigInteger.Three.Multiply(k).Mod(n);
                t = t.Multiply(k.ModInverse(n)).Mod(n);
                Console.WriteLine(t);
            }
            catch
            {
                Console.WriteLine("+");
                k = BigInteger.ProbablePrime(BitLenght, new Random());
                goto A;
            }
            
            
            d = e.ModInverse(q_1.Multiply(p_1));
            
        }
    }
    class Program
    {

        static void Main(string[] args)
        {
            UserRSA alice = new UserRSA();
            UserRSA bob = new UserRSA();
            UserRSA trent = new UserRSA(64);
            

            BigInteger Ra = BigInteger.ProbablePrime(32, new Random());
            Console.WriteLine();
            Console.WriteLine("Alice: Ra: " + Ra);
            Console.WriteLine();

            Thread.Sleep(100);
            Console.WriteLine();
            Console.WriteLine("Alice -> Bob: a.id: " + alice.id + " Ra: " + Ra);
            Console.WriteLine();




            BigInteger Rb = BigInteger.ProbablePrime(32, new Random());
            Console.WriteLine();
            Console.WriteLine("Bob: Rb: " + Rb);
            Console.WriteLine();


            BigInteger message = new BigInteger(Encoding.ASCII.GetBytes(alice.id+"."+Ra.ToString() + "." + Rb.ToString()));

            BigInteger EbMessage = message.ModPow(bob.e, bob.n);

            Console.WriteLine();
            Console.WriteLine("Bob->Trent: b.id: " + bob.id + " Eb(a.id,Ra,Rb): " + EbMessage);
            Console.WriteLine();

            BigInteger K = new BigInteger(Encoding.ASCII.GetBytes(trent.k.ToString()+"."+ trent.n.ToString()));
            Console.WriteLine();
            Console.WriteLine("Trent: K: " + K);
            Console.WriteLine();

            BigInteger DtMessage = EbMessage.ModPow(bob.d, bob.n);
            //Console.WriteLine(DtMessage);

            //Console.ReadKey();

            BigInteger AIdDtMessage = new BigInteger(Encoding.ASCII.GetString(DtMessage.ToByteArray()).Split('.')[0]);
            BigInteger RaDtMessage = new BigInteger(Encoding.ASCII.GetString(DtMessage.ToByteArray()).Split('.')[1]);

            BigInteger RbDtMessage = new BigInteger(Encoding.ASCII.GetString(DtMessage.ToByteArray()).Split('.')[2]);

            BigInteger trentMessage1 = new BigInteger(Encoding.ASCII.GetBytes(bob.id.ToString()+"."+ K.ToString()+"."+RaDtMessage.ToString()+"."+RbDtMessage.ToString()));

            BigInteger trentMessage2 = new BigInteger(Encoding.ASCII.GetBytes(AIdDtMessage.ToString()+"."+K.ToString()));

            
            

            BigInteger EaTrentMessage1 = trentMessage1.ModPow(alice.e, alice.n);

            BigInteger EbTrentMessage2 = trentMessage2.ModPow(bob.e, bob.n);
            Console.WriteLine();
            Console.WriteLine("Trent->Alice: Ea(b.id, K, Ra, Ra): " + EaTrentMessage1);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Trent->Alice: Eb(a.id, K): " + EbTrentMessage2);
            Console.WriteLine();
            BigInteger Da = EaTrentMessage1.ModPow(alice.d, alice.n);

            //Console.WriteLine("Ra + Rb + K :"  + Encoding.ASCII.GetString(Da.ToByteArray()));
            BigInteger RaDa = new BigInteger(Encoding.ASCII.GetString(Da.ToByteArray()).Split('.')[2]);


            if (Ra.ToString() == RaDa.ToString())
            {
                Console.WriteLine();
                Console.WriteLine("Ra соответствует ранее переданному.");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Ra не соответствует ранее переданному.");
                Console.WriteLine();
            }

            BigInteger RbDa = new BigInteger(Encoding.ASCII.GetString(Da.ToByteArray()).Split('.')[3]);

           // Console.WriteLine("Rb :" + RbDa);

            BigInteger T = new BigInteger(Encoding.ASCII.GetString(Da.ToByteArray()).Split('.')[1]);

           // Console.WriteLine("K :"+T);

            BigInteger Tk = new BigInteger(Encoding.ASCII.GetString(T.ToByteArray()).Split('.')[0]);
            
            BigInteger Tn = new BigInteger(Encoding.ASCII.GetString(T.ToByteArray()).Split('.')[1]);

            
          //  Console.WriteLine("Kk :" + Tk);
         //   Console.WriteLine("Kn :"+ Tn);

           

          //  Console.WriteLine("Rb :" + RbDa);

            BigInteger Et = RbDa.Multiply(Tk).Mod(Tn);

            //  Console.WriteLine(Et);
            Console.WriteLine();
            Console.WriteLine("Alice->Bob: Eb(a.id, K): " + EbTrentMessage2);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Alice->Bob: Ek(Rb): " + Et);
            Console.WriteLine();
            //BigInteger Ek = Rb.ModPow(trent.e, trent.n);

            BigInteger Db = EbTrentMessage2.ModPow(bob.d, bob.n);

            BigInteger TDb = new BigInteger(Encoding.ASCII.GetString(Db.ToByteArray()).Split('.')[1]);

            //Console.WriteLine(TkDb);
            BigInteger TkDb = new BigInteger(Encoding.ASCII.GetString(TDb.ToByteArray()).Split('.')[0]);
            BigInteger TnDb = new BigInteger(Encoding.ASCII.GetString(TDb.ToByteArray()).Split('.')[1]);

            //Console.WriteLine(TnDb);

            BigInteger DEt = Et.Multiply(TkDb.ModInverse(TnDb)).Mod(TnDb);
            Console.WriteLine();
            Console.WriteLine("K: " + TDb.ToString());
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Rb from Alice: " + DEt.ToString());
            Console.WriteLine();
            if (DEt.ToString() == Rb.ToString())
            {
                Console.WriteLine();
                Console.WriteLine("Проверка прошла успешна!!");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Проверка не прошла");
                Console.WriteLine();
            }
            


            //Console.WriteLine(K.Subtract(Ra).Subtract(Rb));

            //BigInteger DbEk = Ek.ModPow(trent.d, trent.n);

            //Console.WriteLine(DbEk);





            Console.ReadKey();



        }
    }
}
