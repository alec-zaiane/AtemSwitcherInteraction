﻿/* -LICENSE-START-
** Copyright (c) 2019 Blackmagic Design
**
** Permission is hereby granted, free of charge, to any person or organization
** obtaining a copy of the software and accompanying documentation covered by
** this license (the "Software") to use, reproduce, display, distribute,
** execute, and transmit the Software, and to prepare derivative works of the
** Software, and to permit third-parties to whom the Software is furnished to
** do so, all subject to the following:
** 
** The copyright notices in the Software and this entire statement, including
** the above license grant, this restriction and the following disclaimer,
** must be included in all copies of the Software, in whole or in part, and
** all derivative works of the Software, unless such copies or derivative
** works are solely in the form of machine-executable object code generated by
** a source language processor.
** 
** THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
** IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
** FITNESS FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO EVENT
** SHALL THE COPYRIGHT HOLDERS OR ANYONE DISTRIBUTING THE SOFTWARE BE LIABLE
** FOR ANY DAMAGES OR OTHER LIABILITY, WHETHER IN CONTRACT, TORT OR OTHERWISE,
** ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
** DEALINGS IN THE SOFTWARE.
** -LICENSE-END-
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using BMDSwitcherAPI;

namespace SimpleSwitcher
{
	class Program
	{
		static long GetInputId(IBMDSwitcherInput input)
		{
			input.GetInputId(out long id);
			return id;
		}

		static void Main(string[] args)
		{
			// Create switcher discovery object
			IBMDSwitcherDiscovery discovery = new CBMDSwitcherDiscovery();
			

			// Connect to switcher
			Console.Write("Enter switcher IP address: ");
            String switcherIP = "192.168.1.2";//Console.ReadLine();

			discovery.ConnectTo(switcherIP, out IBMDSwitcher switcher, out _BMDSwitcherConnectToFailure failureReason);
			Console.WriteLine("Connected to switcher");

			var atem = new AtemSwitcher(switcher);

			// Get reference to various objects
			IBMDSwitcherMixEffectBlock me0 = atem.MixEffectBlocks.FirstOrDefault();
			IBMDSwitcherKey switcher_key = atem.switcher_keyers.FirstOrDefault();
			IBMDSwitcherTransitionParameters me0TransitionParams = me0 as IBMDSwitcherTransitionParameters;
			IBMDSwitcherTransitionWipeParameters me0WipeTransitionParams = me0 as IBMDSwitcherTransitionWipeParameters;
			IBMDSwitcherInput input3 = atem.SwitcherInputs
										.Where((i, ret) => {
											i.GetPortType(out _BMDSwitcherPortType type);
											return type == _BMDSwitcherPortType.bmdSwitcherPortTypeExternal;
										})
										.ElementAt(4);


			long prevProgramId;
            //me0.GetProgramInput(out prevProgramId);
			//


			switcher_key.GetInputFill(out prevProgramId);

			long programId;

			while (true)
			{

                //switcher_key.GetInputFill(out programId);
                switcher_key.GetInputFill(out programId);
                Console.WriteLine(programId);

                if (prevProgramId != programId)
                {
                    //Trigger Camera Change in Unreal
                    //Console.WriteLine(programId);


                    if (programId == 8 && prevProgramId == 7) break;

					prevProgramId = programId;
					//theoretically this is where we send outputs instead of sending to console ._. -Sam
					if (programId == 1)
					{
						SendKeys.SendWait("{9}");
					}   
					if(programId == 3)
                    {
						SendKeys.SendWait("{2}");
                    }

	

				}

			}

			Console.Write("Press ENTER to exit...");
			Console.ReadLine();
		}
	}

	internal class AtemSwitcher
	{
		private IBMDSwitcher switcher;

		public AtemSwitcher(IBMDSwitcher switcher) => this.switcher = switcher;

		public IEnumerable<IBMDSwitcherMixEffectBlock> MixEffectBlocks
		{
			get
			{
				// Create a mix effect block iterator
				switcher.CreateIterator(typeof(IBMDSwitcherMixEffectBlockIterator).GUID, out IntPtr meIteratorPtr);
				IBMDSwitcherMixEffectBlockIterator meIterator = Marshal.GetObjectForIUnknown(meIteratorPtr) as IBMDSwitcherMixEffectBlockIterator;
				if (meIterator == null)
					yield break;

				// Iterate through all mix effect blocks
				while (true)
				{
					meIterator.Next(out IBMDSwitcherMixEffectBlock me);

					if (me != null)
						yield return me;
					else
						yield break;
				}
			}
		}

		public IEnumerable<IBMDSwitcherKey> switcher_keyers
        {
			get
            {
                var atem = new AtemSwitcher(switcher);
                IBMDSwitcherMixEffectBlock me0 = atem.MixEffectBlocks.FirstOrDefault();
                me0.CreateIterator(typeof(IBMDSwitcherKeyIterator).GUID, out IntPtr KeyPtr);
                IBMDSwitcherKeyIterator key_iterator = Marshal.GetObjectForIUnknown(KeyPtr) as IBMDSwitcherKeyIterator;
				if (key_iterator == null)
				{
					yield break;
				}
                while(true)
                {
                    key_iterator.Next(out IBMDSwitcherKey key);
                    if(key != null)
                    {
                        yield return key;
                    }
                    else
                    {
                        yield break;
                    }

                }
		
            }

        }

		public IEnumerable<IBMDSwitcherInput> SwitcherInputs
		{
			get
			{
				// Create an input iterator
				switcher.CreateIterator(typeof(IBMDSwitcherInputIterator).GUID, out IntPtr inputIteratorPtr);
				IBMDSwitcherInputIterator inputIterator = Marshal.GetObjectForIUnknown(inputIteratorPtr) as IBMDSwitcherInputIterator;
				if (inputIterator == null)
					yield break;

				// Scan through all inputs
				while (true)
				{
					inputIterator.Next(out IBMDSwitcherInput input);

					if (input != null)
						yield return input;
					else
						yield break;
				}
			}
		}
	}
}
