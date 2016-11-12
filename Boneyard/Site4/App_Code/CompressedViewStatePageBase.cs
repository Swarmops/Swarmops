using System;
using System.IO;
using System.IO.Compression;
using System.Collections;
using System.ComponentModel;
using System.Web.UI;
using System.Configuration;
using System.Threading;
using System.Globalization;
using System.Text; 
using System.Data;

public static class CompressViewState
{
    public static byte[] Compress(byte[] data)
    {
        MemoryStream output = new MemoryStream();
        GZipStream gzip = new GZipStream(output,
                          CompressionMode.Compress, true);
        gzip.Write(data, 0, data.Length);
        gzip.Close();
        return output.ToArray();
    } 

    public static byte[] Decompress(byte[] data)
    {
        MemoryStream input = new MemoryStream();
        input.Write(data, 0, data.Length);
        input.Position = 0;
        GZipStream gzip = new GZipStream(input,
                          CompressionMode.Decompress, true);
        MemoryStream output = new MemoryStream();
        byte[] buff = new byte[64];
        int read = -1;
        read = gzip.Read(buff, 0, buff.Length);
        while (read > 0)
        {
            output.Write(buff, 0, read);
            read = gzip.Read(buff, 0, buff.Length);
        }
        gzip.Close();
        return output.ToArray();
    }
} 


public class CompressedViewStatePageBase : PageV4Base
{
    private ObjectStateFormatter _formatter = 
        new ObjectStateFormatter(); 

    protected override void
        SavePageStateToPersistenceMedium(object viewState)
    {
        MemoryStream ms = new MemoryStream();
        _formatter.Serialize(ms, viewState);
        byte[] viewStateArray = ms.ToArray();
        ClientScript.RegisterHiddenField("__COMPRESSEDVIEWSTATE",
            Convert.ToBase64String(
            CompressViewState.Compress(viewStateArray)));
    }
    protected override object
        LoadPageStateFromPersistenceMedium()
    {
        string vsString = Request.Form["__COMPRESSEDVIEWSTATE"];
        byte[] bytes = Convert.FromBase64String(vsString);
        bytes = CompressViewState.Decompress(bytes);
        return _formatter.Deserialize(
            Convert.ToBase64String(bytes));
    }
}
