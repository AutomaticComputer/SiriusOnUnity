# SiriusOnUnity

This is an emulator for Ferranti Sirius computer on Unity. 

Sirius is a small transistor computer from UK company Ferranti, announced in 1959. 
Its programs and outputs were punched on paper tapes, just like the earliest "automatic computers" 
such as [EDSAC(YouTube)](https://www.youtube.com/watch?v=6v4Juzn10gM&feature=youtu.be).

It was a decimal machine, not uncommon those days. 
It was not a fast computer, but it cost less than other computers of the time, 
and it also came with a (somewhat) high-level language called Autocode. 
Compared with the earliest computers such as EDSAC and Manchester Mark I, 
its instruction set looks quite modern, 
with 9 registers (1 control register = program counter, 
and 8 general purpose registers) each useable as an index register, 
a subroutine call instruction with any of 8 registers as a link register, etc. 

Other (relatively) small computers of that era include Elliott 803(UK), IBM 1620, IBM 1401, PDP-1 (US), 
NEAC 2203 (Japan), etc. 

About 20 were built, not a great number compared with some of other computers as above, 
but it is an interesting machine, seemingly easy to use at least for simple tasks, I think. 

For more on Sirius (and Autocode), see: 

- Computer Conservation Society, [Ferranti Sirius](https://www.computerconservationsociety.org/software/sirius/base.htm)
- Wikipedia: [Ferranti Sirius](https://en.wikipedia.org/wiki/Ferranti_Sirius)
- Barbara Ainsworth, [Monash University's First Computer](http://citeseerx.ist.psu.edu/viewdoc/download;jsessionid=380CEDE9D0A74FE7D714EC512C379E75?doi=10.1.1.726.9778&rep=rep1&type=pdf)
- J. W. Ponton, [The Autocodes: Sirius to IMP, a User's Perspective](http://www.homepages.ed.ac.uk/jwp/history/autocodes/)

Especially, the first link contains a gate-level simulator of Sirius, programming documents and programs. 

## This emulator

![Screenshot](screenshot.png).

I wanted to know what it is like to program a computer with a teleprinter and  paper tapes, 
so I made this emulator. 

You can type into the teleprinter(teletype) to punch a paper tape, 
feed it to the computer, get the result punched on a tape 
and then print it with the teleprinter. 

The emulator is based on information from "LD11 Sirius Programming Manual." 
When I couldn't decide on certain details (e.g. the behavior of the OVR flag) from the manual, 
I checked them on the simulator. 


### Restrictions/Differences: 

- It runs all programs that came with the simulator, 
but I don't think it works 100% correctly. 
Where it differs from the simulator, I believe the simulator is right. 

- Visually, it is not so realistic, as you see from the screenshot. 
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

- The behavior of indicators seems to be a little different from the simulator. 
(For example, in the manual mode, "WORD != 0" etc. works immediately after 
pressing "Continue" in this emulator, but in the simulator it works after one more press.)

- I have never used a Sirius (or anything before microcomputers), 
so there will be many inaccuracies. 
The layout of the keyboard on the teleprinter is almost certainly wrong. 

- I am not sure if the behavior of "slowing down mode" is correct. 
(Actually, this emulator has "overclock" mode, which the real one of course didn't.) 

- You can speed up the printing with the "10x" button. 

### Installation

Just unpack the [binary](SiriusOnUnity_Windows.zip) 
or build it yourself. 

Or, the [WebGL version](https://automaticcomputer.github.io/SiriusOnUnity/SiriusOnUnity_WebGL/index.html) 
might work. 
In this case, you can only try "ptw" files (see below), 
and there is no way to retrieve the tapes and printouts. 

### Trying the emulator

After you start up the program, you can use arrow keys and PageUp/PageDown 
to move the camera around. 
You can also move/zoom by sliding/pinching on touch screens. 

I coded and punched a few sample programs. 
To try them, follow these steps: 

1. Push the "LOAD" button on the leftmost tape reader. 
This brings up several program tapes. 
Choose "HelloWorld.ptw", for example.

2. Make sure that "Manual" and "WAIT" keys in the Control Box (main console) are pressed, 
and push the "PRIMARY INPUT" button. 
The number in the first row of figures is the current order(instruction) being obeyed. 
The second row displays the content of the register selected by the leftmost column of keys. 
The register 1 is the "Control Register"(program counter), 
which in this case holds the location where the program is being stored. 

3. When the loading stops, push "CLEAR CONTROL", press "AUTO" and then "RUN". 
A number of holes are punched (printed, actually) on the tape in the right of the computer. 
When Sirius stops with "99 Wait" indicator on, 
push "SAVE" button on the tape punch. 
It saves the data into "(User directory)\AppData\LocalLow\AutomaticComputer\SiriusOnUnity\Tapes\\" 
as "yyyyMMddHHmmss.ptp" (yyyyMMddHHmmss is the current date and time, 
and if a file with this name exists, an extra number is appended). 

4. Push the "LOAD" button on the teleprinter, click on the tape, 
press the "PUNCH" button on the teleprinter to suppress punching, 
and then press the "READ" button. 
The contents of the tape are read and printed. 
You can save the output as an image with "CUT" button. 
The data is saved in 
"(User directory)\AppData\LocalLow\AutomaticComputer\SiriusOnUnity\Printouts\\" as "yyyyMMddHHmmss.png". 

5. To quit, just close the window. 

You can try other tapes with the extension "ptw". 

- Mandelblot.ptw draws a Mandelblot set via the tape punch. 
If you are not patient enough, you can click on the left of the wheel in the bottom 
to overclock the machine. 
- Mandelblot_pr_pi.ptw draws a Mandelblot set directly to the teleprinter. 
- Exponential_PI.ptw calculates 1000 digits of Napier's number exp(1). 
It is coded in a straightforward way, and there must be much room for improvement. 
The register 5 holds the index for the expansion of exp(1), which counts up to 500.  
- Lunar_pi.ptw is the Lunar Lander game, based closely on 
[Jim Storer's original program](https://www.cs.brandeis.edu/~storer/LunarLander/LunarLander.html). 
It is only "approximately correct", probably because of my sloppy use of the fixed point arithmetic. 
To play, run it as above, 
and when the emulated Sirius stops with the prompt "K=?" on the printer, 
compose the fuel rate (8-200 or 0) on the 3 least significant digits on the control box, 
and push "CONTINUE". 
I don't think it was usual to play a game on a real Sirius in those days. 


Now, "ptw" files are in the bare machine code format 
(you can print it with the teleprinter), 
and this is not how ordinary programs were written. 
A program in machine code 
was usually written in a more convenient form, which allows the use of "parameters", for example. 
You can see an example by printing out "Exponential.ptr", "Lunar_mc.ptr", or "Mandelblot_mc.ptr". 
Such files were loaded by the "Initial Order" program. 

Or, you could write a program in "Autocode". 
As examples, 
- "Mandelblot_ac.ptr" draws (punches) a Mandelblot set. This takes about 2 hours. 
You can save and print the result during the calculation. 
- "Lunar_ac.ptr" is a port of Jim Storer's original Lunar Lander code in FOCAL on PDP-8. 
It gives almost the same result as the original one, 
and took much less time to code than "Lunar_pi.ptw" or "Lunar_mc.ptr". 
- "Hamurabi_ac.ptr" is a port of David H. Ahl's version of 
[Hamurabi (Wikipedia)](https://en.wikipedia.org/wiki/Hamurabi_(video_game))

"Initial Order" and "Autocode", along with other programs and document, 
are distributed along with the simulator from the Computer Conservation Society. 
The instruction on the simulator should be mostly applicable to this emulator. 

When you write a program, note the following: 
- The tape readers are on the input channels 0 and 5. 
- The tape punch is on the output channel 0. 
- The teleprinter is on the input channel 1 and the output channel 1, 
although I don't think it was common to connect a teleprinter directly 
(because it would have lead to a waste of machine cycles). 
- In this emulator, Sirius has 10000 words of store(memory), but the Initial Order and Autocode 
that come with the simulator are for a 4000 words Sirius. 
For small programs, it won't matter. 
- You can use "teleprinter" program that comes with the simulator. 
In case you type a program using the teleprinter in this emulator, 
it has minimal capability for correcting a tape: 
You can load a tape, read a character and copy it to another tape or skip a character. 
You can also connect tapes in a similar way. 
- You can add, delete or rename tapes in 
"(User directory)\AppData\LocalLow\AutomaticComputer\SiriusOnUnity\Tapes\\". 
The tapes that come with the emulator are (re)written every time 
a new (or different) version is launched(it looks at "Tapes\version"). 

In teleprinter, you can type from the keyboard of the PC. 
As in "teleprinter" program for the simulator, ther are substitutes 
for those keys that are not on the PC: 
- for LF, TAB (you need Enter + TAB to finish a line), 
- for Letter Shift/Figure Shift, right/left Ctrl
- for "Blank tape", '|'
- for "Greater than or equal to", '}'
- for "Not equal to", '#'
- for "Arrow", '^'
- for "Multiply", 'x'
- for (backspace and) "Erase", Back Space


### Some utility programs 

Actually, there is only one at this moment. 

"PITranslate.ptr" allows one to write a program for Primary Input 
using Initial Orders. 

Write a program that starts with address 0. 
(Note that location 2 is where interrupt handling starts, 
and location 1 is where the return address for the interrupt is stored. 
So the first order will be a jump to a convenient location. 
When you don't use Interrupt, location 2 might be a good choice.)
End the program with the warning character "L". 
Other warning characters won't work. 
The last location in the program must be the highest location in the program. 
The program can have up to 100 parameters. 

Load Initial Orders, load this program using Initial Orders, 
set the program tape prepared as above and run "PITranslate" with "Continue". 
This program rewrites part of Initial Orders so that the program is loaded 
at the locations from 400, 
and punches the resulting code to the output tape. 
(It rewrites back the original code if successful, but if unsuccessful the result is unknown.)


## Plans

Things I might do in the future: 

- Write documents on programming on Sirius, including a tutorial. 
- Clean up the code and assets. 
For example, the internal BCD code currently used is a little different from the one in the real Sirius, 
so I might fix this. 
- Build up a "personal" "mini-computer like" computing environment: 
Something like TECO, another game, tiny basic perhaps, etc. 
Of course, this is an anachronistic project. 
- Try to reconstruct a "Library Tape" from the subroutines contained in Autocode Compiler, 
together with "Assembly programme". 
- Another old computer --- ETL Mark IV or parametron computer PC-1 from Japan, perhaps. 

## Acknowledgement

The documents and the simulator from the Computer Conservation Society was essential 
in developing this emulator. 

"Linux Libertine" and "Biolinum" fonts, 
licensed under the SIL Open Font License, Version 1.1, 
are used on buttons and the indicator panel. 

## Contact

Write as an issue or send me an e-mail, 
but since this is a holiday project, it may take a week or more before I respond. 


## License

[Unlicense](https://unlicense.org) applies to the current version. 
(Other licenses might apply to future versions.)
