
open System
open System.Numerics
open System.Collections.Generic
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running


type StructuralEqualityRecord =
    { Distance: float32
      Normal: Vector4 }

[<ReferenceEquality>]
type ReferentialEqualityRecord =
    { Distance: float32
      Normal: Vector4 }

type Lookups () =

    static let (structuralData: StructuralEqualityRecord list) =
        [{Distance=1.0f; Normal = Vector4.UnitW}
         {Distance=3.5f; Normal = Vector4.UnitX}
         {Distance=1.7f; Normal = Vector4.UnitY}]

    static let (referentialData: ReferentialEqualityRecord list) =
        [{Distance=1.0f; Normal = Vector4.UnitW}
         {Distance=3.5f; Normal = Vector4.UnitX}
         {Distance=1.7f; Normal = Vector4.UnitY}]

    static let structuralSetData =
        HashSet<StructuralEqualityRecord>()

    static let referentialSetData =
        HashSet<ReferentialEqualityRecord>()

    static let structuralLinkedList =
        LinkedList<StructuralEqualityRecord>()

    static let referentialLinkedList =
        LinkedList<ReferentialEqualityRecord>()

    static let structuralToFind =
        structuralData.[1]

    static let referentialToFind =
        referentialData.[1]

    
    [<Benchmark>]
    member _.FetchStructuralHashSet () =
        structuralSetData.Contains(structuralToFind)

    [<Benchmark>]
    member _.FetchReferentialHashSet () =
        referentialSetData.Contains(referentialToFind)

    [<Benchmark>]
    member _.FetchStructuralLinkedList () =
        structuralLinkedList.Contains(structuralToFind)

    [<Benchmark>]
    member _.FetchReferentialLinkedList () =
        referentialLinkedList.Contains(referentialToFind)

    [<Benchmark>]
    member _.FetchStructuralList () =
        structuralData |> List.contains structuralToFind

    [<Benchmark>]
    member _.FetchReferentialList () =
        referentialData |> List.contains referentialToFind
    
        

[<EntryPoint>]
let main argv =
    ignore <| BenchmarkRunner.Run<Lookups>()

    0