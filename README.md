# yaio
# Yet Another Image Organizer
This is a C# WPF project that moves images from one folder to another and also creates in the destination folder a [year] and [01] [02] and so on for each month

# MIT License

Copyright (c) 2021 luc1pop

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. 


## How to use this

![Main window](https://github.com/luc1pop/yaio/blob/main/assets/mainwindow.png)

* Process folder - enter here the path to the folder where the image files are located.
* Target folder - enter here the path to the folder where you want the image to be sorted
* Month folder sufix - enter here if there should be a suffix for the month folder. For example "wedding" will create "2004/03 wedding/" as target for all the images taken 2004 in March in the process folder
* Settings 
    * Create years folder - controls if a year folder should it be created
    * Create months folder  - controls if a month sub folder should it be created
    * Recursive - controls if the program should search recursively for files in "Process folder".
    * Delete duplicates from process folder - if a file is already in the "Target folder" the the file from the process folder will be removed. 
    * Recursive search for duplicates in year folder - for each file in the process folder the target folder with that year will be searched recursively.
* File filter - comma-separated list for file endings.
* [Start moving] - click here to start process
* [Stop] - click here to stop


## What it does
When running this on this folder:
![Process](https://github.com/luc1pop/yaio/blob/main/assets/source.png)

will move all the images and sort them to 

![Process](https://github.com/luc1pop/yaio/blob/main/assets/target.png)

