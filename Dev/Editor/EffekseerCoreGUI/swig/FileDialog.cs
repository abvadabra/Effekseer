//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.2
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace Effekseer.swig {

public class FileDialog : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal FileDialog(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(FileDialog obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~FileDialog() {
    Dispose(false);
  }

  public void Dispose() {
    Dispose(true);
    global::System.GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          EffekseerNativePINVOKE.delete_FileDialog(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public static string OpenDialog(string filterList, string defaultPath) {
    string ret = System.Runtime.InteropServices.Marshal.PtrToStringUni(EffekseerNativePINVOKE.FileDialog_OpenDialog(filterList, defaultPath));
    return ret;
  }

  public static string SaveDialog(string filterList, string defaultPath) {
    string ret = System.Runtime.InteropServices.Marshal.PtrToStringUni(EffekseerNativePINVOKE.FileDialog_SaveDialog(filterList, defaultPath));
    return ret;
  }

  public static string PickFolder(string defaultPath) {
    string ret = System.Runtime.InteropServices.Marshal.PtrToStringUni(EffekseerNativePINVOKE.FileDialog_PickFolder(defaultPath));
    return ret;
  }

  public FileDialog() : this(EffekseerNativePINVOKE.new_FileDialog(), true) {
  }

}

}