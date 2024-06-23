Imports System
Imports System.Text
Imports System.IO
Imports System.IO.Compression


Module Program

    Private br As BinaryReader
    Private des As String
    Private source As String
    Private buffer As Byte()

    <Obsolete>
    Sub Main(args As String())

        If args.Count = 0 Then
            Console.WriteLine("UnPack Tool - 2CongLc.Vn")
        Else
            source = args(0)
        End If

        If File.Exists(source) Then

            br = New BinaryReader(File.OpenRead(source))

            br.BaseStream.Position = 6
            Dim offset As Int64 = br.ReadInt64
            br.BaseStream.Position = offset
            br.BaseStream.Position += 22
            Dim count As Int32 = br.ReadInt32
            Dim subfiles As New List(Of FileData)()

            For i As Int32 = 0 To count - 1
                subfiles.Add(New FileData)
                Dim unknow As Int64 = br.ReadInt64
            Next

            des = Path.GetDirectoryName(source) + "\" + Path.GetFileNameWithoutExtension(source) + "\"
            Directory.CreateDirectory(des)

            Dim n As Int32 = 0
            For Each fd As FileData In subfiles
                Console.WriteLine("File Offset : {0} - File Size : {1} - File Name : {2}", fd.offset, fd.size, n)
                br.BaseStream.Position = fd.offset
                Dim buffer As Byte() = br.ReadBytes(fd.size)
                Using bw As New BinaryWriter(File.Create(des + n))
                    bw.Write(buffer)
                End Using

                br = New BinaryReader(File.OpenRead(des + n))
                Dim sign As String = New String(Encoding.UTF7.GetString(br.ReadBytes(4)))
                If sign = "Game" Or sign = ";Game" Then
                    sign += New String(Encoding.UTF7.GetString(br.ReadBytes(9)))
                End If
                br.Close()

                If sign = "\u0089PNG" Then
                    File.Move(des + "\" + n, des + "\" + n + ".png")
                ElseIf sign = "DDS " Then
                    File.Move(des + "\" + n, des + "\" + n + ".dds")
                ElseIf sign = "Gamebryo File" Then
                    File.Move(des + "\" + n, des + "\" + n + ".nif")
                ElseIf sign = ";Gamebryo KFM" Then
                    File.Move(des + "\" + n, des + "\" + n + ".kfm")
                Else
                    File.Move(des + "\" + n, des + "\" + n + ".unknow")
                End If
                n += 1
            Next

            br.Close()
            Console.WriteLine("Unpack done !!!")
        End If
        Console.ReadLine()
    End Sub

    ' cấu trúc dữ liệu block
    Class FileData
        Public unknown As Byte = br.ReadByte 'usually 0x4000. Sometimes 0x2000.
        Public offset As Int64 = br.ReadInt64
        Public size As Int32 = br.ReadInt32
    End Class
End Module
