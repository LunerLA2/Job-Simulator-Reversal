using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace TwitchChatter
{
	internal class TwitchChatStream
	{
		private readonly int C_MAX_CHAT_LINE_COUNT = 100;

		private TcpClient m_tcp_client;

		private StreamReader m_in_stream;

		private StreamWriter m_out_stream;

		private string[] m_chat_msg_buffer;

		private int m_chat_write_cursor;

		private int m_chat_read_cursor;

		private bool m_kill_thread;

		private bool m_is_connected;

		public void start(string ip, int port)
		{
			if (!m_is_connected)
			{
				m_chat_msg_buffer = new string[C_MAX_CHAT_LINE_COUNT];
				for (int i = 0; i < C_MAX_CHAT_LINE_COUNT; i++)
				{
					m_chat_msg_buffer[i] = "";
				}
				Thread thread = new Thread((ThreadStart)delegate
				{
					stream_async_func(ip, port);
				});
				thread.IsBackground = true;
				thread.Start();
			}
		}

		public List<string> get_new_twitch_messages()
		{
			return get_lines_from_buffer(m_chat_msg_buffer, ref m_chat_read_cursor, m_chat_write_cursor);
		}

		public bool get_is_connected()
		{
			if (m_is_connected)
			{
				if (m_tcp_client != null)
				{
					return m_tcp_client.Connected;
				}
				return false;
			}
			return false;
		}

		public void submit_message(string message)
		{
			m_out_stream.WriteLine(message);
			m_out_stream.Flush();
		}

		public void submit_messages(List<string> messages)
		{
			foreach (string message in messages)
			{
				m_out_stream.WriteLine(message);
			}
			m_out_stream.Flush();
		}

		public void stop()
		{
			if (m_is_connected && !m_kill_thread)
			{
				m_kill_thread = true;
				if (m_tcp_client != null)
				{
					m_tcp_client.Close();
				}
			}
		}

		private void stream_async_func(string ip, int port)
		{
			try
			{
				write_chat_msg(TwitchChatMessageTokenizer.C_TWITCH_CHATTER_STRING + " Connecting to " + ip + "...");
				m_tcp_client = new TcpClient(ip, port);
				m_in_stream = new StreamReader(m_tcp_client.GetStream());
				m_out_stream = new StreamWriter(m_tcp_client.GetStream());
				write_chat_msg(TwitchChatMessageTokenizer.C_TWITCH_CHATTER_STRING + " Connected to " + ip + "!");
				m_is_connected = true;
				string msg;
				while (!m_kill_thread && (msg = m_in_stream.ReadLine()) != null)
				{
					write_chat_msg(msg);
				}
				write_chat_msg(TwitchChatMessageTokenizer.C_TWITCH_CHATTER_STRING + " Stream has ended at " + ip + ".");
			}
			catch (Exception ex)
			{
				write_chat_msg(TwitchChatMessageTokenizer.C_TWITCH_CHATTER_STRING + " Stream exception on " + ip + " " + ex.Message);
			}
			finally
			{
				write_chat_msg(TwitchChatMessageTokenizer.C_TWITCH_CHATTER_STRING + " Closing connection for " + ip + "...");
				if (m_out_stream != null)
				{
					m_out_stream.Close();
				}
				m_out_stream = null;
				if (m_in_stream != null)
				{
					m_in_stream.Close();
				}
				m_in_stream = null;
				m_tcp_client = null;
				m_is_connected = false;
			}
		}

		private List<string> get_lines_from_buffer(string[] buffer, ref int read_cursor, int write_cursor)
		{
			int num = read_cursor;
			List<string> list = new List<string>();
			for (int i = num; i < write_cursor; i++)
			{
				list.Add(string.Copy(buffer[i]));
			}
			read_cursor = write_cursor;
			return list;
		}

		private void write_chat_msg(string msg)
		{
			write_msg_to_buffer(msg, m_chat_msg_buffer, ref m_chat_write_cursor);
		}

		private void write_msg_to_buffer(string msg, string[] buffer, ref int cursor)
		{
			buffer[cursor] = msg;
			cursor++;
			if (cursor >= buffer.Length)
			{
				cursor = 0;
			}
		}
	}
}
