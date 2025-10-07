using System;

namespace zencontrol.Sdk.Tpi;

/// <summary>
/// Represents a response from a TPI controller, containing answer type,
/// answer content, and checksum validation.
/// </summary>
public class TpiReply : IModeCommand
{
    /// <summary>
    /// The Type of answer from the controller.
    /// </summary>
    public AnswerType? Answer { get; private set; }

    /// <summary>
    /// Contents of the answer from the controller.
    /// </summary>
    public byte? AnswerByte { get; private set; }

    /// <summary>
    /// Checksum of the packet, does not determine if it is correct or not.
    /// </summary>
    public byte? CheckSum { get; private set; }

    /// <summary>
    /// Creates a null TPI reply (used when no response is received)
    /// </summary>
    /// <param name="isNull">Must be true to create a null reply</param>
    /// <exception cref="TpiException">Thrown when isNull is false</exception>
    public TpiReply(bool isNull = true)
    {
        if (isNull)
        {
            Answer = null;
            AnswerByte = null;
            CheckSum = null;
        }
        else
        {
            throw new TpiException("This method shall only be used when the packet is Null");
        }
    }

    /// <summary>
    /// Creates a TPI reply from individual components
    /// </summary>
    /// <param name="answerType">Type of answer from the controller</param>
    /// <param name="answerByte">Content of the answer</param>
    /// <param name="checkSum">Checksum of the reply packet</param>
    public TpiReply(byte answerType, byte answerByte, byte checkSum) : this(new byte[]
        { answerType, answerByte, checkSum })
    {
    }

    /// <summary>
    /// Creates a TPI reply from raw byte data
    /// </summary>
    /// <param name="data">3-byte array containing answer type, answer byte, and checksum</param>
    /// <exception cref="TpiException">Thrown for invalid data length, checksum mismatch, or invalid answer type</exception>
    public TpiReply(byte[] data)
    {
        if (data.Length != 3)
        {
            throw new TpiException("Invalid TpiReply");
        }

        Answer = (AnswerType)data[0];
        AnswerByte = data[1];
        CheckSum = data[2];
        if (((byte)Answer ^ AnswerByte) != CheckSum)
        {
            throw new TpiException("Invalid TpiReply Packet, Checksum does not match");
        }


        if (!Enum.IsDefined(typeof(AnswerType), Answer))
        {
            throw new TpiException("Invalid TpiReply, Invalid AnswerType");
        }
    }

    /// <summary>
    /// Converts the TPI reply instance into a byte array representation.
    /// </summary>
    /// <returns>A 3-byte array containing the answer type, answer byte, and checksum.
    /// Returns an empty array if the reply is considered null.</returns>
    public byte[] ToByteArray()
    {
        if (Answer == null) return [];
        if (AnswerByte == null) return [];
        if (CheckSum == null) return [];

        return new byte[] { (byte)Answer, AnswerByte.Value, CheckSum.Value };
    }

    /// <summary>
    /// Simple string for TpiReply 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        if (AnswerByte is null)
            return "NULL";
        if (Answer is null)
            return "NULL";
        if (CheckSum is null)
            return "NULL";

        return $"{Answer},{AnswerByte},{CheckSum} (0x{(byte)Answer:X2}{((byte)AnswerByte):X2}{(byte)CheckSum:X2})";
    }

    /// <summary>
    /// Types of answers that can be received from the TPI controller
    /// </summary>
    public enum AnswerType : byte
    {
        /// <summary>Command executed successfully without data response</summary>
        ReplyOk = 0x50,

        /// <summary>Command executed successfully with data response</summary>
        ReplyAnswer = 0x51,

        /// <summary>No response received from target device</summary>
        ReplyNoAnswer = 0x52,

        /// <summary>Error occurred during command execution</summary>
        ReplyError = 0x53,
    }
}