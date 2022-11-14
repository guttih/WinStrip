#!/bin/bash

#true or false options.
options=( "-h" "--help" "-clean" "-source=" ) 

#Options that must be provided by the user
# optionsRequired=( "-clean"  "-source=" ) 

#Set to true you want to allow any arguments to be given
#Set to false if you only want to allow options in  "options" and "optionsWithArgument"
ALLOW_UNPROCESSED="true" 

printHelp() {
    printf 'Usage: %s [OPTIONS]...\n' "$(basename "$0")"
    printf 'Usage: %s [OPTIONS]... (-source=<Value>)\n' "$(basename "$0")"
    echo "  What does you script do?"
    echo 
    echo "OPTIONS         Option description"
    echo "  --help        Prints this help page"
    echo "  -clean    What does clean represent? "
    echo "  -source    What does source represent? "
    echo
    echo "ARGUMENTS     Option argument description"
    echo " sourceValue     Describe Value in more detail!"
    echo
    exit 0
}


highlight=$(echo -en '\033[01;37m')
errorColor=$(echo -en '\033[01;31m')
warningColor=$(echo -en '\033[00;33m')
norm=$(echo -en '\033[0m')

#Function: parseOptions()
#
#Brief: Checks if all options are correct and saves each in a variable.
#After: Value of each options given, is stored in a uppercase named variable.
#       f. example -express will be stored in a global variable called EXPRESS
#Returns:
#      0 : (success) All parameters are valid
#      1 : (error) One or more parameters are invalid
#
declare -a UNPROCESSED
parseOptions() {
    containsElement() { #if function arrayContains exists, it can be used instead of containsElement
        local e match="$1"
        shift
        for e; do [[ "$e" == "$match" ]] && return 0; done
        return 1
    }

    extractArgumentValue() {
        if [ $# -ne 2 ]; then return; fi
        declare CHECK=$2
        declare PREFIX_LEN=${#CHECK}
        PREFIX_LEN=$((PREFIX_LEN + 1)) #Incrementing for equal sign
        if [[ "$1" != "$CHECK"* ]]; then return; fi
        echo "${1:${PREFIX_LEN}}"
    }

    extractArgumentName() {
        if [ $# -ne 1 ]; then return; fi
        if [[ $1 != *"="* ]]; then return; fi;
        declare match="="
        declare prefix=${1%%"$match"*}
        echo "${prefix}"
    }

    declare -a _optionsFound
    declare tmp tmpValue tmpName
    while (("$#")); do # While there are arguments still to be shifted
        tmp="clean"
        tmpValue="true"
        tmpName=$(extractArgumentName "$1")

        if [[ -n "$tmpName" ]] && containsElement "$tmpName=" "${options[@]}"; then
            tmpValue=$(extractArgumentValue "$1" "$tmpName")
            tmp=$tmpName
            tmpName="$tmpName="
            _optionsFound+=("$tmpName")
        elif containsElement "$1" "${options[@]}"; then
            tmp="$1"
            tmpName="$tmp"
            _optionsFound+=("$tmpName")
        elif [[ "$ALLOW_UNPROCESSED" == "true" ]]; then
            UNPROCESSED+=("$1")
            _optionsFound+=("$1")
        else
            echo "${errorColor}Error: ${highlight}$1${norm} is an invalid argument."
            return 1
        fi
        #removing prefix - and -- and assigning value to upper cased variable.

        tmp=${tmp#"-"}                  # removing -
        tmp=${tmp#"-"}                  # removing - if there were two
        tmp=$(echo "$tmp" | tr a-z A-Z) # CAPITALIZING the variable name
        printf -v "$tmp" "$tmpValue"    # Assigning a value to VARIABLE
        if [[ -z "$tmpValue" ]]; then
            echo "Value missing for $tmpName"
            return 1
        fi

        shift
    done
    if [[ -n "$HELP" || -n "$H" ]]; then printHelp; fi
    #Check if all required options have been provided.
    if [[ ${#optionsRequired[@]} -eq 0 ]]; then return 0; fi
    for arg in "${optionsRequired[@]}"; do
        if ! containsElement "$arg" "${_optionsFound[@]}"; then
            echo "${errorColor}Required option missing ${norm} $arg "
            return 1
        fi
    done
}

# You could test code below by running this script with these Arguments
#   ./thisScript.sh -clean -source="~/Downloads /home/gandalf" -weird
if ! parseOptions "$@"; then exit 1; fi
if [[ -n "$CLEAN" ]]; then echo "-clean=\"$CLEAN\""; fi
if [[ -n "$source" ]]; then echo "-source=\"$source\""; fi

for arg in "${UNPROCESSED[@]}"; do
    echo  "${warningColor}Unprocessed argument${norm} $arg "
done
FILE="/home/gudjon/personal/WinStrip/build-LinStrip-Desktop_Qt_6_3_0_GCC_64bit-Debug/LinStrip.SendToSerial"

echo "Testing LinuxStripApp::CheckAndGetExternalCommands()"
# echo "{\"delay\":0,\"com\":2,\"brightness\":1,\"values\":[0,0,0],\"colors\":[16711935,16711680,32768,255,16777215,10824234]}" >>$FILE
echo "{\"delay\":0,\"com\":3,\"brightness\":5,\"values\":[4,0,0],\"colors\":[16711935,16711680,32768,255,16777215,10824234]}" >>$FILE