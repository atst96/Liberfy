namespace Liberfy.ViewModels.Authentications
{
    /// <summary>
    /// 認証失敗メッセージ
    /// </summary>
    internal class AuthenticationFailedMessage
    {
        /// <summary>
        /// 要約
        /// </summary>
        public string Instruction { get; set; }

        /// <summary>
        /// メッセージ
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// <see cref="AuthenticationFailedMessage"/>を生成する
        /// </summary>
        public AuthenticationFailedMessage()
        {
        }

        /// <summary>
        /// <see cref="AuthenticationFailedMessage"/>を生成する
        /// </summary>
        /// <param name="instruction">要約</param>
        /// <param name="message">メッセージ</param>
        public AuthenticationFailedMessage(string instruction, string message)
        {
            this.Instruction = instruction;
            this.Message = message;
        }
    }
}
