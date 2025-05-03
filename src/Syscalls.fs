module Syscalls

[<RequireQualifiedAccess>]
type Platform =
    | RARS
    | Linux

type Definition = Definition of name: string * number: int * args: List<Type.Type> * ret: Type.Type

let rarsSyscalls =
    [ Definition("PrintInt", 1, [ Type.TInt ], Type.TUnit)
      Definition("PrintFloat", 2, [ Type.TFloat ], Type.TUnit)
      Definition("PrintDouble", 3, [ Type.TFloat ], Type.TUnit)
      Definition("PrintString", 4, [ Type.TString ], Type.TUnit)
      Definition("ReadInt", 5, [], Type.TInt)
      Definition("ReadFloat", 6, [], Type.TFloat)
      Definition("ReadDouble", 7, [], Type.TFloat)
      Definition("ReadString", 8, [ Type.TString ], Type.TUnit)
      Definition("Sbrk", 9, [ Type.TInt ], Type.TInt)
      Definition("Exit", 10, [], Type.TUnit)
      Definition("PrintChar", 11, [ Type.TString ], Type.TUnit)
      Definition("ReadChar", 12, [], Type.TString)
      Definition("GetCWD", 17, [ Type.TString; Type.TInt ], Type.TUnit)
      Definition("Time", 30, [], Type.TInt)
      Definition("MidiOut", 31, [ Type.TInt ], Type.TUnit)
      Definition("Sleep", 32, [ Type.TInt ], Type.TUnit)
      Definition("MidiOutSync", 33, [ Type.TInt ], Type.TUnit)
      Definition("PrintIntHex", 34, [ Type.TInt ], Type.TUnit)
      Definition("PrintIntBinary", 35, [ Type.TInt ], Type.TUnit)
      Definition("PrintIntUnsigned", 36, [ Type.TInt ], Type.TUnit)
      Definition("RandSeed", 40, [ Type.TInt; Type.TInt ], Type.TUnit)
      Definition("RandInt", 41, [ Type.TInt ], Type.TInt)
      Definition("RandIntRange", 42, [ Type.TInt; Type.TInt ], Type.TInt)
      Definition("RandFloat", 43, [ Type.TInt ], Type.TFloat)
      Definition("RandDouble", 44, [ Type.TInt ], Type.TFloat)
      Definition("ConfirmDialog", 50, [ Type.TString ], Type.TInt)
      Definition("InputDialogInt", 51, [], Type.TInt)
      Definition("InputDialogFloat", 52, [], Type.TFloat)
      Definition("InputDialogDouble", 53, [], Type.TFloat)
      Definition("InputDialogString", 54, [ Type.TString; Type.TString; Type.TInt ], Type.TInt)
      Definition("MessageDialog", 55, [ Type.TString; Type.TInt ], Type.TUnit)
      Definition("MessageDialogInt", 56, [ Type.TString; Type.TInt ], Type.TUnit)
      Definition("Close", 57, [ Type.TInt ], Type.TUnit)
      Definition("MessageDialogDouble", 58, [ Type.TString; Type.TFloat ], Type.TUnit)
      Definition("MessageDialogString", 59, [ Type.TString; Type.TString ], Type.TUnit)
      Definition("MessageDialogFloat", 60, [ Type.TString; Type.TFloat ], Type.TUnit)
      Definition("LSeek", 62, [ Type.TInt; Type.TInt; Type.TInt ], Type.TInt)
      Definition("Read", 63, [ Type.TInt; Type.TString; Type.TInt ], Type.TInt)
      Definition("Write", 64, [ Type.TInt; Type.TString; Type.TInt ], Type.TInt)
      Definition("Exit2", 93, [ Type.TInt ], Type.TUnit)
      Definition("Open", 1024, [ Type.TString; Type.TInt ], Type.TInt) ]

let findSyscall (platform: Platform) (number: int) =
    match platform with
    | Platform.RARS -> rarsSyscalls |> List.tryFind (fun (Definition(_, n, _, _)) -> n = number)
    | _ -> None

let syscallFormatName (platform: Platform) (number: int) =
    match findSyscall platform number with
    | Some(Definition(name, _, _, _)) -> name
    | None -> $"syscall_%d{number}"