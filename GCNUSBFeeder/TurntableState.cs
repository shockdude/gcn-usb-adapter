using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCNUSBFeeder
{
    public class TurntableState
    {
        public bool square;
        public bool cross;
        public bool circle;
        public bool triangle; // also Euphoria
        public bool start;
        public bool select;
        public bool ps;

        public int POVstate;

        public int tableL;
        public int tableR;
        public int effects;
        public int crossfader;

        //public bool gR;
        //public bool rR;
        //public bool bR;
        //public bool gL;
        //public bool rL;
        //public bool bL;

        public TurntableState()
        {
            square = false;
            cross = false;
            circle = false;
            triangle = false;

            start   = false;
            select  = false;
            ps = false;

            POVstate = -1;

            tableL = 128;
            tableR = 128;
            crossfader = 128;
            effects = 128;

            //gR = false;
            //rR = false;
            //bR = false;
            //gL = false;
            //rL = false;
            //bL = false;
        }

        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static TurntableState GetState(ref byte[] input)
        {
            //[0]
            //01 = square
            //02 = cross
            //04 = circle
            //08 = triangle/euphoria
            //[1]
            //01 = select
            //02 = cross
            //10 = ps
            //[2] dpad
            //0F = neutral
            //00 = up
            //02 = right
            //04 = down
            //06 = left
            //[5] table left, centered at 128, lower is counterclockwise, upper is clockwise, probably 6 bits
            //[6] table right, centered at 128, lower is counterclockwise, upper is clockwise, probably 6 bits
            //effects
            //[19] bottom 8 bits
            //[20] upper 4 bits
            //crossfader
            //[21] bottom 8 bits
            //[22] upper 4 bits
            //[23]
            //01 = green right
            //02 = red right
            //04 = blue right
            //10 = green left
            //20 = red left
            //40 = blue left

            TurntableState pad = new TurntableState();
            if (input.Length >= 27)
            {
                byte b = input[0];
                pad.square = (b & (1 << 0)) != 0;
                pad.cross = (b & (1 << 1)) != 0;
                pad.circle = (b & (1 << 2)) != 0;
                pad.triangle = (b & (1 << 3)) != 0;

                b = input[1];
                pad.select = (b & (1 << 0)) != 0;
                pad.start = (b & (1 << 1)) != 0;
                pad.ps = (b & (1 << 4)) != 0;

                b = input[2];
                if (b == 0xFF)
                {
                    pad.POVstate = -1;
                }
                else
                {
                    pad.POVstate = b;
                }

                pad.tableL = Clamp((input[5] - 96) * 4, 0, 255);
                pad.tableR = Clamp((input[6] - 96) * 4, 0, 255);

                pad.effects = ((input[19] & 0xF0) >> 2) | (input[20] << 6);
                pad.crossfader = ((input[21] & 0xF0) >> 2) | (input[22] << 6);

                //b = input[23];
                //pad.gR = (b & (1 << 0)) != 0;
                //pad.rR = (b & (1 << 1)) != 0;
                //pad.bR = (b & (1 << 2)) != 0;
                //pad.gL = (b & (1 << 4)) != 0;
                //pad.rL = (b & (1 << 5)) != 0;
                //pad.bL = (b & (1 << 6)) != 0;

                return pad;
            }
            else
            {
                throw new Exception("Invalid byte array for input");
            }
        }
    }
}
