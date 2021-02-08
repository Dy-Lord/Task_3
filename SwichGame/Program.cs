using System;
using System.Security.Cryptography;
using System.Text;

namespace SwichGame
{
    class Program
    {
        static bool winCheck(string[] args, int userMove,int pcMove)
        {
            int div = (args.Length - 1) / 2;
            if (userMove - div >= 0)
                return (pcMove >= userMove - div && pcMove < userMove) ? true : false;
            else
                return (pcMove <= userMove + div && pcMove > userMove) ? false : true;            
        }
        static string menuSetup(string[] args)
        {
            string temp = "Available moves:\n";

            for (int i = 0; i < args.Length; i++)
                temp += $"--{i + 1}--\t" + args[i] + '\n';
            temp += "--0--\tExit";

            return temp;
        }
        static void Main(string[] args)
        {
            if ((args.Length & 1) == 0 || args.Length < 3)
            {
                Console.WriteLine("Invalid number of moves in parameters.\nEnter 3,5,7 or more moves.\n");
                goto exit;
            }

            foreach (string move in args)
                if (Array.FindAll(args, item => item.Equals(move)).Length >= 2)
                {
                    Console.WriteLine($"Move -{move}- contains duplicates.\nEnter 3,5,7 or more Different moves.\n");
                    goto exit;
                }

            while (true)
            {
                int pcMove;
                byte[] key = new byte[16];
                String xkey;

                using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
                {
                    crypto.GetBytes(key);                 
                    xkey = BitConverter.ToString(key).Replace("-", string.Empty);

                    Random rnd = new Random();
                    pcMove = rnd.Next(0, args.Length - 1);

                    using (HMACSHA256 hmac = new HMACSHA256(Encoding.ASCII.GetBytes(xkey)))
                    {
                        byte[] hashValue = hmac.ComputeHash(Encoding.ASCII.GetBytes(args[pcMove]));
                        Console.WriteLine("HMAC:\n" + BitConverter.ToString(hashValue).Replace("-", string.Empty));
                    }
            }

            while (true)
                {
                    Console.WriteLine(menuSetup(args) + "\nEnter your move:");
                    int userMove = Int32.Parse(Console.ReadLine()) - 1;
                    if (userMove >= args.Length || userMove <-1)
                        Console.WriteLine("Invalid menu item, please try again.\n");
                    else if (userMove == -1)
                        goto exit;
                    else
                    {
                        Console.WriteLine("Your move: " + args[userMove] + '\n' + "Computer move: " + args[pcMove]);
                        if (userMove == pcMove)
                        {
                            Console.WriteLine("Oooops, try again :)\n");
                            break;
                        }
                        else
                        {
                            if (winCheck(args, userMove, pcMove))
                                Console.WriteLine("\nYou win!\n");
                            else
                                Console.WriteLine("\nYou lose!\n");
                            Console.WriteLine("HMAC key: " + xkey + '\n');
                            goto exit;
                        }
                    }        
                }
            }
            exit: Console.WriteLine("Game is closed");
        }
    }
}
