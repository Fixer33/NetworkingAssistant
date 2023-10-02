<h1> This is an automatisation for telegram chat. </h1>
Created for analysis and parsing of question that were already used in the past, exporting these questions into file and tracking registration of participants.

<h2> Commands: </h2>
<ul>
    <li></li>
    <li><b>stop</b> - Stops client and exits application</li>
    <li><b>help</b> - Get list and description of all available commands</li>
    <li><b>show_selected_chat_info</b> - Get info about selected chat</li>
    <li><b>select_all_messages</b> - Select all messages from selected chat. Will print out the amount. You should repeat this command a few times until number of selected messages stops changing</li>
    <li><b>leave_only_questions</b> - Leave only questions in message buffer</li>
    <li><b>generate_questions</b> - Generate messages in chatgpt. Places them into string buffer</li>
    <li><b>get_chats</b> <i>*chat_limit*</i> - Lists all available to current user chats. *chat_limit* set maximum amount of chats listed</li>
    <li><b>select_chat</b> <i>*id*</i> - Selects chat with *id*. Saves in buffer for further operations</li>
    <li><b>export_questions</b> <i>*file_name*</i> - Exports all questions from selected chat into file. No extension needed</li>
    <li><b>send_reg_poll</b> <i>*headline*</i> - Send poll for registration. *headline* will be displayed on poll header. Shows created poll id</li>
</ul>