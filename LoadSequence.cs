using System;
using System.Collections.Generic;
using System.Linq;

namespace Array371A
{

    public class LoadSequence
    {
        byte regMode;
        byte repeating;
        List<ProgramStep> steps;
        byte stepCount;

        public LoadSequence(byte newProgRegMode)
        {
            regMode = newProgRegMode;
            repeating = 0;
            stepCount = 0;
        }

        public LoadSequence(byte newProgRegMode, bool repeat)
        {
            regMode = newProgRegMode;
            if (repeat)
                repeating = 1;
            else
                repeating = 0;
            stepCount = 0;
        }
        public LoadSequence(byte newProgRegMode, bool repeat, List<ProgramStep> sequence)
        {
            regMode = newProgRegMode;
            if (repeat)
                repeating = 1;
            else
                repeating = 0;
            if (sequence.Count() <= 10)
            {
                steps = sequence;
                stepCount = (byte)sequence.Count();
            }
            else
            {
                throw (new ApplicationException("sequence is larger than max allow number of steps"));
            }
        }

        public void AddStep(ProgramStep nextStep)
        {
            if (steps.Count() < 10)
            {
                steps.Add(nextStep);
                stepCount = (byte)steps.Count();
            }
            else
            {
                throw (new ApplicationException("cannot add to sequence that is full"));
            }

        }

        public void AddStep(ushort setValue, ushort time)
        {
            if (steps.Count() < 10)
            {
                steps.Add(new ProgramStep(setValue, time));
                stepCount = (byte)steps.Count();
            }
            else
            {
                throw (new ApplicationException("cannot add to sequence that is full"));
            }
        }

        public byte[] FirstPayload()
        {
            byte[] payloadSteps1To5 = new byte[22];
            
            int i = 0;
            int payloadIndex = 2;
            foreach (ProgramStep step in steps)
                {
                if (i == 5) break;
                Buffer.BlockCopy(step.ToArray(), 0, payloadSteps1To5, payloadIndex, 4);
                i++;
                payloadIndex += 4;
                }
            payloadSteps1To5[0] = regMode;
            payloadSteps1To5[1] = stepCount;
            return payloadSteps1To5;
        }
        public byte[] SecondPayload()
        {
            byte[] payloadSteps6To10 = new byte[22];
            
            int i = 0;
            int payloadIndex = 0;
            foreach (ProgramStep step in steps)
            {
                if (i++ < 5) continue;
                Buffer.BlockCopy(step.ToArray(), 0, payloadSteps6To10, payloadIndex, 4);
                payloadIndex += 4;
            }
            payloadSteps6To10[20] = repeating;
            return payloadSteps6To10;
        }
    }

    public class ProgramStep
    {
        ushort value; // make float and convert correctly ??
        ushort time;

        public ProgramStep(ushort setValue, ushort setTime)
        {
            value = setValue;
            time = setTime;
        }

        public byte[] ToArray()
        {
            byte[] array = new byte[4];
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, array, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(time), 0, array, 2, 2);
            return array;
        }
    }
}
