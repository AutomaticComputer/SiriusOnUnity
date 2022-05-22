# Trying the emulator

Online emulator: [WebGL version](https://automaticcomputer.github.io/SiriusOnUnity/webgl.html). 

In what follows, I often refer to the 
["Official" Simulator](https://www.computerconservationsociety.org/software/sirius/base.htm)
at Computer Conservation Society. 


[Navigating](#navigating)

[First sample programs](#first-sample-programs)

[Renaming, Deleting, Adding and retrieving files](#renaming-deleting-adding-and-retrieving-files)

[Sample programs with Intial Orders and Autocode](#sample-programs-with-intial-orders-and-autocode)

[Writing your own code](#writing-your-own-code)

[Substitute keys](#substitute-keys)



## Navigating

After you start up the program, you can move the camera using
- Arrow keys
- PageUp/PageDown 

On touch screens, you can also use 
- slide/pinch, to move or zoom. 


## First Sample Programs

I coded and punched a few standalone sample programs. 
To try them, follow these steps: 

1. Push the "LOAD" button on the tape reader to the left of the console. 
This brings up several program tapes. 
Choose "HelloWorld.ptw", for example.

2. Make sure that "Manual" and "WAIT" keys in the Control Box (main console) are pressed, 
and push the "PRIMARY INPUT" button. 
The number in the first row of figures on the panel is the current order(instruction) being obeyed. 
The second row displays the content of the register selected by the leftmost column of keys. 
The register 1 is the "Control Register"(program counter), 
which in this case holds the location where the program is being stored. 

3. When the loading stops, push "CLEAR CONTROL", press "AUTO" and then "RUN". 
A number of holes are punched (printed, actually) on the tape punch at the leftmost position. 
When Sirius stops, almost immediately, with "99 Wait" indicator on, 
push "SAVE" button on the tape punch. 
It saves the data as "yyyyMMddHHmmss.ptp" (yyyyMMddHHmmss is the current date and time, 
and if a file with this name exists, an extra number is appended). 

4. Push the "LOAD" button on the teleprinter, click on the tape just saved, 
press the "PUNCH" button on the teleprinter if you want to suppress punching, 
and then press the "READ" button. 
The contents of the tape are read and printed. 
You can save the output as an image with "CUT" button(see below). 


5. To quit, just close the window. 

You can try other tapes with the extension "ptw". 

- Mandelblot_pr_pi.ptw draws a Mandelblot set on the teleprinter. 
If you are not patient enough, you can click on the left of the wheel at the bottom of the Control Box 
to overclock the machine. 
- Exponential_PI.ptw calculates 1000 digits of Napier's number exp(1). 
It is coded in a straightforward way, and there must be much room for improvement. 
The register 5 holds the index for the expansion of exp(1), which counts up to 500.  
- Lunar_pi.ptw is the Lunar Lander game, based closely on 
[Jim Storer's original program](https://www.cs.brandeis.edu/~storer/LunarLander/LunarLander.html). 
It is only "approximately correct", probably because of my sloppy use of the fixed point arithmetic. 
To play it, run the program as above, 
and when the emulated Sirius stops with the prompt "K=?" on the printer, 
compose the fuel rate (8-200 or 0) on the 3 least significant digits on the control box, 
and push "CONTINUE". 
I don't think it was usual to play a game on a real Sirius in those days. 


## Renaming, Deleting, Adding and Retrieving files

To rename or delete a tape, click the "pen" or the "trash can." 

### Standalone version
On the standalone application, 
tapes and printouts are stored in the "persistent data path" as defined by Unity. 
On Windows, they go to "(User directory)\AppData\LocalLow\AutomaticComputer\SiriusOnUnity\Tapes\\" 
and 
"(User directory)\AppData\LocalLow\AutomaticComputer\SiriusOnUnity\Printouts\\". 
To add a tape, just copy a file into the above directory. 

The tapes that come with the emulator are (re)written every time 
a new (or different) version is launched(it looks at "Tapes\version"). 

### WebGL version

On the WebGL version, 
tapes and printouts are stored in the memory. 
(So, regretfully, they are lost each time you leave.) 
To add a tape, click on the file selection button at the bottom of the webpage. 
To retrieve a tape click the "postbox." 
When a tape is selected, a download link is added at the bottom of the page. 
Similarly, "CUT" button on the teleprinter 
adds a download link for the printout. 


## Sample programs with Intial Orders and Autocode

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

To show Hamurabi_ac.ptr, for example, 
- Add "Autocode Compiler.ptw" to the tape library as in the previous section, 
and load it with the "PRIMARY INPUT" button. 
- Attach "Hamurabi_ac.ptr" to the first (i.e. next to the Control box) tape reader. 
Push "CLEAR CONTROL", press "AUTO" and then "RUN". 
- When the computer stops, press "CONTINUE". 
- The program stops for randomization. 
Compose 6 digits on the Control box (least significant digits), and "CONTINUE". 
- The game starts now. 
The program accepts 4 digits from the Control box. 


## Writing your own code

You can write your own programs using the teleprinter. 
Note the following: 
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


### Substitute keys

In teleprinter, you can type from the keyboard of the PC. 
As in "teleprinter" program for the simulator, there are substitutes 
for keys missing on the PC: 
- for LF, TAB (you need Enter + TAB to finish a line), 
- for Letter Shift/Figure Shift, right/left Ctrl
- for "Blank tape", '|'
- for "Greater than or equal to", '}'
- for "Not equal to", '#'
- for "Arrow", '^'
- for "Multiply", 'x'
- for (backspace and) "Erase", Back Space

I don't know if the behavior of "Letter Shift/Figure Shift" on this emulator is correct. 
If you do, please let me know. 
