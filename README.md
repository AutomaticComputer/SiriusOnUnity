# SiriusOnUnity

This is an emulator for Ferranti Sirius computer on Unity. 

You can try it online: [WebGL version](https://automaticcomputer.github.io/SiriusOnUnity/webgl.html). 

[About this emulator](#this-emulator)

[Restrictions and Differences](#restrictions-and-differences)

[Installation](#installation)

[Trying the emulator](first.md)


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

![Screenshot1](screenshot1.png).

![Screenshot2](screenshot2.png).

I wanted to know what it is like to program a computer with a teleprinter and  paper tapes, 
so I made this emulator. 

You can type into the teleprinter(teletype) to punch a paper tape, 
feed it to the computer, get the result punched on a tape 
and then print it with the teleprinter. 

The emulator is based on information from "LD11 Sirius Programming Manual." 
When I couldn't decide on certain details (e.g. the behavior of the OVR flag) from the manual, 
I checked them on the simulator. 


### Restrictions and Differences

- It seems to run all programs that came with the "official" simulator, 
but I don't think it works 100% correctly. 
Where it differs from the simulator, I believe the simulator is right. 

- Visually, it is not so realistic, as you see from the screenshot. 
For example, 
tapes and printed paper are cylindrical, 
keyboards are placed almost vertically, and so on. 
This is basically because of my lack of time, artistic talent, 
knowledge of Unity etc. 
(Some of them are due to design decisions. 
I know that paper tape readers/punchs didn't look like this, 
but this was convenient for my purpose.) 

- As I am just a hobby programmer, the code might be somewhat strange. 

- In this emulator, the unit of execution is an order(instruction), not a clock cycle. 
Further, it executes a number of orders each frame. 
Thus indicators "A-state" and "C-state" do not work (they are unused), 
and playing music is not possible yet (sound is not implemented yet, anyway). 

- The behavior of indicators seems to be a little different from the simulator. 
(For example, in the manual mode, "WORD != 0" etc. works immediately after 
pressing "Continue" in this emulator, but in the simulator it works after one more press.)

- I have never used a Sirius (or anything before microcomputers), 
so there will be many inaccuracies. 
The layout and the behavior of the keyboard on the teleprinter are almost certainly wrong. 

- I am not sure if the behavior of "slowing down mode" is correct. 
(Actually, this emulator has "overclock" mode, which the real one of course didn't.) 

- You can speed up the printing with the "10x" button. 


### Installation

Just unpack the [binary](SiriusOnUnity_Windows.zip) or build it yourself. 

Or, you can try [WebGL version](https://automaticcomputer.github.io/SiriusOnUnity/webgl.html). 


### Trying the emulator

[Trying the emulator](first.md)


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
