# SiriusOnUnity

This is an emulator for Ferranti Sirius computer on Unity. 

Sirius is a small transistor computer from UK company Ferranti, announced in 1959. 
Its programs and outputs were punched on paper tapes, just like the earliest "automatic computers." 
It was a decimal machine, not uncommon those days. 
It was not a fast computer, but its cost was low, and it also came with a (slightly) high-level language called Autocode. 

Other (relatively) small computers of that era includes Elliott 803(UK), IBM 1620, IBM 1401, PDP-1 (US), 
NEAC 2203 (Japan), etc. 

About 20 were built, a small number compared with other computers above, 
but it is an interesting machine I think. 

For more on Sirius (and Autocode), see: 

- Computer Conservation Society, [Ferranti Sirius](https://www.computerconservationsociety.org/software/sirius/base.htm)
- Wikipedia: [Ferranti Sirius](https://en.wikipedia.org/wiki/Ferranti_Sirius)
- Barbara Ainsworth, [Monash University's First Computer](http://citeseerx.ist.psu.edu/viewdoc/download;jsessionid=380CEDE9D0A74FE7D714EC512C379E75?doi=10.1.1.726.9778&rep=rep1&type=pdf)
- The Autocodes: [Sirius to IMP, a User's Perspective](http://www.homepages.ed.ac.uk/jwp/history/autocodes/)

Especially, the first link contains a gate-level simulator of Sirius, programming documents and programs. 

## This emulator

I wanted to know what it is like to program a computer in the era of teleprinters and  paper tapes, 
so I made this emulator. 

You can type into the teleprinter(teletype) to punch a paper tape, 
feed it to the computer, get the results punched on a tape 
and then print it with the teleprinter. 

### Restrictions: 

- It runs all programs that came with the simulator, 
but I don't think it works 100% correctly. 
Where it differs from the simulator, I believe the simulator is right. 

- Visually, it is not so realistic: [Screenshot](screenshot.png)
For example, the whole system floats in the air, 
tapes and printed paper are cylindrical, 
keyboards are placed vertically, and so on. 
They are basically because of my lack of time, artistic talent, 
knowledge of Unity etc. 
(Some of them are due to design decisions. 
I know that paper tape readers/punchs didn't look like this, 
but this was convenient for my purpose.) 

- As I am just a hobby programmer, the code might be somewhat strange. 

- In this emulator, the unit of execution is an order(instruction), not a clock cycle. 
Further, it executes a number of orders each frame. 
Thus indicators "A-state" and "C-state" do not work (they are unused), 
and playing music is not possible (sound is not implemented yet, anyway). 

- The behavior of indicators seem to be a little different from the simulator. 
(For example, in the manual mode, "WORD != 0" etc. works immediately after 
pressing "Continue" in this emulator, but in the simulator it works after one more press.)

- I have never used a Sirius (or anything other than a microcomputer), 
so there will be many inaccuracies. 
The layout of the keyboard on the teleprinter is almost certainly wrong. 

- Speeding down not yet implemented. 


### Trying the emulator

After you start up the program, you can use arrow keys and PageUp/Down 
to move the camera around. 

I coded and punched a few small sample programs. 
To try them, follow these steps: 

1. Push the "LOAD" button on the leftmost tape reader. 
This brings up several program tapes. 
Choose "HelloWorld.ptw", for example.

2. Make sure that "Manual" and "WAIT" keys in the main console(Control Box) are pressed. 
and push the "PRIMARY INPUT" button. 
The number in the first row of figures is the current order(instruction) being obeyed. 
If you push "1" in the leftmost column, 
you can see in the second row the current "Control Register"(program counter), 
which shows where the program is being stored. 

3. When it stops, press "CLEAR CONTROL", "AUTO" and then "RUN". 
A number of holes are punched (printed, actually) on the tape in the right of the computer. 
When Sirius stops with "99 Wait" indicator on, 
push "SAVE" button on the tape punch. 
It saves the data into "SiriusOnUnity_Data/Tapes/" 
as "yyyyMMddHHmmss.ptp" (yyyyMMddHHmmss is the current date and time). 


4. Push the "LOAD" button on the teleprinter, 
and click on (or touch) the tape 
press the "PUNCH" button on the teleprinter to suppress punching, 
and then press the "READ" button. 
The contents of the tape are read and printed. 
You can save the output with "CUT" button. 
The data is saved in "SiriusOnUniti_Data/Printouts/" as "yyyyMMddHHmmss.png". 


You can try other tapes with the extension "ptw": 

- Mandelblot.ptw draws a Mandelblot set via the tape punch. 
- Mandelblot_pr_pi.ptw draws a Mandelblot set directly to the teleprinter. 
- Exponential_PI.ptw calculates 1000 digits of Napier's number. 
It is coded in a rather straightforward way, and there must be much room for improvement. 

In fact, "ptw" files are in bare machine code format 
(you can print it with the teleprinter), 
and this is not how ordinary programs were written. 
If a program is in machine code, 
it was written in a more convenient form, which allows the use of labels, for example. 
You can see an example by printing out "Mandelblot_mc.ptr". 
It was loaded by "Initial Order" program. 

Otherwise, you could write a program in "Autocode": 
See "Mandelblot_ac.ptr" for an example code. 

"Initial Order" and "Autocode", along with other programs and document, 
are distributed along with the simulator from the Computer Conservation Society. 
The instruction on the simulator should be mostly applicable to this emulator. 

If you code yourself, note the following: 
- The tape readers are on input channels 0 and 5. 
- Tape punch is on input channel 0. 
- The teleprinter is on input channel 1 and output channel 1. 
I don't know if it was common to connect a teleprinter directly. 
- In this emulator, Sirius has 10000 words of store(memory), but the Initial Order and Autocode 
that comes with the simulator can use only 4000 words (if I understand it correctly). 
- You can use "teleprinter" program that comes with the simulator. 
In case you type a program using the teleprinter in this emulator, 
it has minimal capability for correcting a tape: 
You can load a tape, read a character and copy it to another tape or skip a character. 
You can also connect tapes in a similar way. 


### Some utility programs 

Actually, there is only one at this moment. 

"PILoader.ptr" allows one to write a a program to be used as Primary Input 
using Initial Orders. 

Write a program that starts with address 0. 
(Note that location 2 is the location where interrupt handling starts, 
and location 1 is where the return address for the interrupt is stored. 
So the first order will be a jump to a convenient location. 
When you don't use Interrupt, location 2 might be a good choice.)
End the program with the warning character "L". 
Other warning characters won't work. 
The last location in the program must be the highest location in the program. 

Load Initial Orders, load this program using Initial Orders, 
set the program tape prepared as above and execute "PILoader" with "Continue". 
This program rewrites part of Initial Order so that the program is loaded 
at the locations from 300, 
and punches the resulting code to the output tape. 
(It rewrites back the original code if successful, but if unsuccessful the result is unknown.)


### Plans

Things I might do in future: 

- Clean up the code and assets. 
For example, the internal BCD code currently used is a little different from the one in the real Sirius, 
so I might fix this. 
- Build up a "personal" computing environment: 
Something like TECO, games, tiny basic perhaps, etc. 
Of course, this is somewhat anachronistic. 
- Another old computer --- ETL Mark IV or parametron computer PC-1 from Japan, perhaps. 


### Contact


Since this is a holiday project, it may take a week or more 
before I respond. 
