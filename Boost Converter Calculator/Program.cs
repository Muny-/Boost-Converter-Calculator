using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boost_Converter_Calculator
{
    public class Variable
    {
        public decimal Val;
        public bool Explicit = false;

        public Variable(bool exp) { this.Explicit = exp; }

        public Variable(decimal value)
        {
            this.Val = value;
            Explicit = true;
        }
    }

    class Program
    {
        static Dictionary<string, Variable> IV = new Dictionary<string, Variable>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "Pin", new Variable(false) },

            { "Vin_min", new Variable(true) },
            { "Vin_max", new Variable(true) },

            { "Iin", new Variable(false) },

            { "n", new Variable(85m) },

            { "k", new Variable(false) },
            { "f", new Variable(true) },

            { "Pout", new Variable(true) },
            { "Vout", new Variable(true) },
            { "Iout", new Variable(false) },

            { "%dIL", new Variable(30m) },
            { "dIL", new Variable(false) },
            { "ILpk", new Variable(false) },

            { "L", new Variable(false) },

            { "Vin_steps", new Variable(10) }
        };

        static bool quit = false;

        static void Main(string[] args)
        {
            // debugging helper
            /*IVS("Pout", 300);
            IVS("Vout", 120);
            IVS("Vin_min", 11.7m);
            IVS("Vin_max", 12.9m);
            IVS("L", 15m);
            IV["L"].Explicit = true;
            IV["%dIL"].Explicit = false;
            IV["f"].Explicit = false;*/
            //

            SetColors.SetColor(ConsoleColor.DarkBlue, System.Drawing.Color.Orange);

            gray();

            wl("Interactive Boost Converter Calculator");

            help();

            while(!quit)
            {
                darkgray();
                w("[]>");
                gray();

                string input = Console.ReadLine();

                HandleInput(input);
            }
        }

        static void help()
        {
            wl("=======================================================================================================================");
            wl(" lv     == Print a list of the names of values used in the calculator, along with a brief explanation");
            wl(" print  == Print a table of the values stored in memory");
            wl(" calc   == Calculate values in the output table, then display the table");
            wl(" calc_r == Calculate values in the output table with Vin incrementing from Vin_min to Vin_max, then display the table");
            wl(" quit   == Quit the program");
            wl(" help   == Display this help message");
            wl("=======================================================================================================================");
        }

        static void lv()
        {
            wl("=======================================================================================================================");
            wl(" Pout  ==   [Watts] Maximum output power of boost circuit.");
            wl(" Vout  ==   [Volts] Output voltage of boost circuit.  The calculator assumes that this is fixed.");
            wl(" Iout  ==   [Amperes] Maximum output current of boost circuit.");
            wl(" n     ==   [Percent] Efficiency of boost circuit.");
            wl(" f     ==   [Hertz] Frequency of PWM signal driving boost circuit's switch.");
            wl(" k     ==   [Percent] Duty cycle of PWM signal driving boost circuit's switch.");
            wl(" Ton   ==   [Seconds] Duration of on-stage in boost circuit.");
            wl(" Pin   ==   [Watts] Maximum input power feeding boost circuit.");

            wl(" Iin   ==   [Amperes] Maximum input current feeding boost circuit. One of the factors that determine the switch,                              inductor, and diode used in the circuit.");
            wl(" %dIL  ==   [Percent] Percentage which specifies the desired ripple current through the boost circuit's inductor.");
            wl(" dIL   ==   [Amperes peak-to-peak] Magnitude of ripple current through inductor.");
            wl(" ILpk  ==   [Amperes] Maximum current that will flow through inductor. Useful for choosing the inductor used.");
            wl(" L     ==   [Henries] Inductance of inductor in boost circuit.");
            wl(" Vin_min == [Volts] Minimum input voltage feeding boost circuit. Considered the worst case scenario for input state.");
            wl(" Vin_max == [Volts] Maximum input voltage feeding boost circuit.");
            wl(" Vin_steps == [Number] When using 'calc_r', the number of times to increment Vin from Vin_min to Vin_max. Affects                              resolution of the voltage-step in each iteration.");
            wl("=======================================================================================================================");
        }

        static void printValues()
        {
            wl("=======================================================================================================================");
            w("[ ");

            orange();
            w("CALCULATED");

            sep();

            magenta();
            w("EXPLICIT");

            sep();

            w("n ");

            darkgray();
            w("= target efficiency");

            sep();

            w("k ");

            darkgray();
            w("= duty");

            sep();

            w("[");

            darkgray();

            w("#");

            gray();

            w("]");

            darkgray();
            w(" = can be explicit OR calculated");

            gray();
            wl(" ]");

            Console.ForegroundColor = ConsoleColor.Cyan;

            wl("                         ALL CALCULATIONS HERE ASSUME Vin = Vin_min");

            gray();

            wl("=======================================================================================================================\n|");

            

            green();

            wl("| OUTPUTS");
            w("|   ");

            gray();

            pVal("Pout", "W");

            sep();

            pVal("Vout", "V");

            sep();

            pVal("Iout", "A");

            wl("\n|");

            green();

            wl("| EFFICIENCY");
            w("|   ");

            gray();

            pVal("n", "%");

            wl("\n|");

            green();

            wl("| SWITCHING");
            w("|   ");

            gray();

            pVal("f", "[1+]KHz");

            sep();

            pVal("k", "%");

            wl("\n|");

            green();

            wl("| INPUTS");
            w("|   ");

            gray();

            pVal("Pin", "W");

            sep();

            pVal("Vin_min", "V");

            w(" - ");

            pVal("Vin_max", "V");

            sep();

            pVal("Iin", "A");

            wl("\n|");

            green();

            wl("| INDUCTOR");
            w("|   ");

            gray();

            pVal("%dIL", "[1-]%");

            sep();

            pVal("dIL", "Apk-pk");

            sep();

            pVal("ILpk", "A");

            wl("");

            w("|   ");

            pVal("L", "[1+]uH");

            wl("\n|");

            green();

            wl("| RANGE CALCULATION");
            w("|   ");

            gray();

            pVal("Vin_steps", "steps");

            wl("\n=======================================================================================================================\n");
        }

        static void sep()
        {
            gray();
            w(" | ");
        }

        static void pVal(string key, string unit)
        {
            white();
            w(key);
            gray();
            w("=");

            color_exp(key);

            w(Math.Round(IVV(key), 4));

            darkgray();

            w(unit);

            gray();

            
        }

        static bool exp(string key)
        {
            return IV[key].Explicit;
        }

        static void color_exp(string key)
        {
            if (!IV[key].Explicit)
                orange();
            else
                magenta();
        }

        static void green()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
        }

        static void yellow()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
        }

        static void darkgray()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }

        static void magenta()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
        }

        static void red()
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }

        static void orange()
        {
            // actually orange
            Console.ForegroundColor = ConsoleColor.DarkBlue;
        }

        static void gray()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        static void white()
        {
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void HandleInput(string input)
        {
            input = input.Replace(" ", "");

            if (input.Contains("="))
            {
                string[] parts = input.Split('=');

                if (parts.Length == 2)
                {
                    string var = parts[0];

                    string value = parts[1];

                    if (IV.ContainsKey(var))
                    {
                        if (IV[var].Explicit || var.ToLower() == "l" || var.ToLower() == "f" || var.ToLower() == "%dil")
                        {
                            if (value == "")
                            {
                                // RESET VALUE
                                IVS(var, 0);
                            }
                            else
                            {
                                decimal result;

                                if (Decimal.TryParse(value, out result))
                                {
                                    var = var.ToLower();

                                    IVS(var, result);

                                    if (var == "l")
                                    {
                                        if (IV["%dIL"].Explicit)
                                            IV["f"].Explicit = false;
                                        else
                                        {
                                            if (IV["f"].Explicit)
                                                IV["%dIL"].Explicit = false;
                                            else
                                                IV["%dIL"].Explicit = true;
                                        }

                                        IV["l"].Explicit = true;
                                    }
                                    else if (var == "f")
                                    {
                                        if (IV["L"].Explicit)
                                            IV["%dIL"].Explicit = false;
                                        else
                                            IV["%dIL"].Explicit = true;

                                        IV["f"].Explicit = true;
                                    }
                                    else if (var == "%dil")
                                    {
                                        if (IV["f"].Explicit)
                                            IV["L"].Explicit = false;
                                        else if (IV["L"].Explicit)
                                            IV["f"].Explicit = false;

                                        IV["%dil"].Explicit = true;
                                    }
                                }
                                else
                                {
                                    wl("Unable to parse value!");
                                }
                            }
                        }
                        else
                        {
                            wl("'" + var + "' is not an explicit input.");
                        }
                    }
                    else
                    {
                        wl("Unknown input variable!");
                    }
                }
                else
                {
                    wl("Too many ='s!");
                }
            }
            else
            {
                switch(input)
                {
                    case "calc_r":

                        decimal vin_min = IVV("vin_min");
                        decimal vin_max = IVV("vin_max");
                        decimal vin_steps = IVV("Vin_steps");
                        

                        if (Calculate(IVV("Vin_min"), false))
                        {
                            wl("=======================================================================================================================");
                            w("[ ");

                            orange();
                            w("CALCULATED");

                            sep();

                            magenta();
                            w("EXPLICIT");

                            sep();

                            w("n ");

                            darkgray();
                            w("= target efficiency");

                            sep();

                            w("k ");

                            darkgray();
                            w("= duty");

                            gray();
                            wl(" ]");

                            wl("=======================================================================================================================");

                            pVal("Pin", "W");
                            sep();
                            pVal("Pout", "W");
                            sep();
                            pVal("Vout", "V");
                            sep();
                            pVal("Iout", "A");
                            sep();
                            pVal("n", "%");

                            wl("");

                            wl("=======================================================================================================================");

                            decimal vin_vstep = (vin_max - vin_min) / (IVV("vin_steps"));

                            for (int i = 0; i < vin_steps-1; i++)
                            {
                                decimal vin = (i * vin_vstep) + vin_min;

                                Calculate(vin, true);

                                if (vin != vin_max && i == vin_steps-2)
                                    Calculate(vin_max, true);
                            }
                        }

                        break;
                    case "calc":
                        if (Calculate(IVV("Vin_min"), false))
                            printValues();
                        break;

                    case "print":
                            printValues();
                        break;

                    case "help":
                        help();
                        break;

                    case "lv":
                        lv();
                        break;

                    case "quit":
                        quit = true;
                        break;

                    default:

                        if (IV.ContainsKey(input))
                        {
                            KeyValuePair<string, Variable> kvp = IV.First(v => v.Key.ToLower() == input.ToLower());
                            wl(kvp.Key + "=" + kvp.Value.Val);
                        }
                        else
                            wl("Unknown expression!");
                        break;
                }
            }
        }

        static bool set_results = true;

        static void Calculate_Range()
        {
            
        }

        static bool Calculate(decimal Vin, bool print)
        {
            set_results = true;

            decimal Pin = IVV("Pin");
            decimal Pout = IVV("Pout");
            decimal Vout = IVV("Vout");
            decimal Iout = IVV("Iout");
            decimal Iin = IVV("Iin");
            decimal per_dIL = IVV("%dIL");
            decimal dIL = IVV("dIL");
            decimal ILpk = IVV("ILpk");
            decimal k = IVV("k");
            decimal f = IVV("f");
            decimal L = IVV("L");
            decimal n = IVV("n");

            if (Pout <= 0)
                ezero("Pout");
            else
            {
                if (Vout == 0)
                    eqzero("Vout");
                else
                {
                    Iout = Pout / Vout;

                    if (n <= 0)
                        ezero("n");
                    else
                    {

                        Pin = Pout / (n / 100m);

                        if (Vin == 0)
                            eqzero("Vin_min or Vin_max");
                        else
                        {
                            k = (1-((Vin*(n/100m))/Vout))*100m;

                            Iin = Pin / Vin;

                            if (exp("%dIL") && per_dIL <= 0)
                                ezero("%dIL");
                            else
                            {
                                if (exp("%dIL"))
                                {
                                    dIL = Iin * (per_dIL / 100m);

                                    if (exp("f"))
                                    {
                                        if (f <= 0)
                                            ezero("f");
                                        else
                                        {
                                            L = ((Vin * k) / ((f * 1000m) * dIL)) * 10000m;
                                        }
                                    }
                                    else if (exp("L"))
                                    {
                                        f = ((Vin * k) / (dIL * (L / 10000))) / 1000m;
                                    }
                                }
                                else
                                {
                                    if (exp("L") && exp("f"))
                                    {
                                        if (f <= 0)
                                            ezero("f");
                                        else
                                        {
                                            if (L <= 0)
                                                ezero("L");
                                            else
                                            {
                                                dIL = ((Vin * k) / ((f * 1000m) * (L / 10000)));

                                                per_dIL = (dIL / Iin) * 100m;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        err("L AND f must be explicit when %dIL is not explicit.");
                                    }
                                }
                                

                                ILpk = Iin + (dIL / 2m);
                            }
                        }
                    }
                }
            }

            if (set_results || print)
            {
                IVS("Pin", Pin);
                IVS("Pout", Pout);
                IVS("Vout", Vout);
                IVS("Iout", Iout);
                IVS("Iin", Iin);
                IVS("%dIL", per_dIL);
                IVS("dIL", dIL);
                IVS("ILpk", ILpk);
                IVS("k", k);
                IVS("f", f);
                IVS("L", L);
            }

            if (print)
            {
                //Vin = 0 V | Iin = 0 A | f = 0 KHz | k = 0 % | dIL = 0 Apk - pk | ILpk = 0 A | L = 0 uH

                w("Vin=");

                yellow();

                w(Math.Round(Vin, 4));

                darkgray();

                w("V");

                gray();

                sep();
                pVal("Iin", "A");
                sep();
                pVal("f", "KHz");
                sep();
                pVal("k", "%");
                sep();
                pVal("dIL", "Apk-pk");
                sep();
                pVal("ILpk", "A");
                sep();
                pVal("%dIL", "%");
                sep();
                pVal("L", "uH");

                wl("");

                wl("=======================================================================================================================");
            }

            return set_results;
        }

        static void eqzero(string var)
        {
            err(var + " must not equal 0.");
        }

        static void ezero(string var)
        {
            err(var + " must be greater than 0.");
        }

        static void err(string msg)
        {
            set_results = false;

            red();

            wl("Error! " + msg);

            gray();
        }

        static void w(object line, params object[] o)
        {
            Console.Write(line.ToString(), o);
        }

        static void wl(object line, params object[] o)
        {
            w(line + "\n", o);
        }

        static decimal IVV(string key)
        {
            return IV[key].Val;
        }

        static bool IVE(string key)
        {
            return IV[key].Explicit;
        }

        static decimal IVS(string key, decimal value)
        {
            IV[key].Val = value;

            return value;
        }
    }
}
