<h1> This is an automatization for telegram chat. </h1>
Created for analysis and parsing of question that were already used in the past, exporting these questions into file and tracking registration of participants.

<h2> Commands: </h2>
<ul>
    <li><b>stop</b> - Stops client and exits application</li>
    <li><b>help</b> - Get list and description of all available commands</li>
    <li><b>show_selected_chat_info</b> - Get info about selected chat</li>
    <li><b>select_all_messages</b> - Select all messages from selected chat. Will print out the amount. You should repeat this command a few times until number of selected messages stops changing</li>
    <li><b>leave_only_questions</b> - Leave only questions in message buffer</li>
    <li><b>generate_questions</b> - Generate messages in chatgpt. Places them into string buffer</li>
    <li><b>get_chats</b> <i>*chat_limit*</i> - Lists all available to current user chats. <i>*chat_limit*</i> set maximum amount of chats listed</li>
    <li><b>select_chat</b> <i>*id*</i> - Selects chat with <i>*id*</i>. Saves in buffer for further operations</li>
    <li><b>export_questions</b> <i>*file_name*</i> - Exports all questions from selected chat into file. No extension needed</li>
    <li><b>send_reg_poll</b> <i>*headline*</i> - Send poll for registration. <i>*headline*</i> will be displayed on poll header. Shows created poll id</li>
    <li><b>select_last_reg_poll</b> - Selects last registration poll in selected messages</li>
    <li><b>send_text_to_poll_users</b> <i>*text*</i> - Send <i>*text*</i> to all users who voted first two options in selected registration poll</li>
</ul>

<h2> How to use: </h2>
<ol>
    <li>Open config.xml (if there is no such file, launch .exe and close it - it will appear) </li>
    <li>Insert telegram api id and has (can be found here https://my.telegram.org/apps) </li>
    <li>IF you want to use chat gpt, insert gpt key </li>
    <li>Launch .exe and follow instructions</li>
    <li>After message "Client started" you can use commands. help - for list of all commands </li>
</ol>

<h2>Download latest build <a href="https://github.com/Fixer33/NetworkingAssistant/releases/tag/TestBuild">here</a> </h2>