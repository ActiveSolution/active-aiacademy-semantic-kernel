# Lesson 7: Create a Multi Agent application with Semantic Kernel and Azure AI Agent service

In this lesson, we will see how Semantic Kernel kan be used, together with Azure AI Agent Service, to develop a multi-agent systems where multiple agents collaborate on a commont task.

1. Ensure all [pre-requisites](pre-reqs.md) are met and installed.

1. Switch to Lesson 7 directory:

    ```bash
    cd ../Lesson7
    ```

1. Start by copying `appsettings.json` from Lesson 1:

    ```bash
    cp ../Lesson1/appsettings.json .
    ```

1. Run the program :

    ```bash
    dotnet run
    ```

1. You should see a conversation between two agents (Software Developer Agent and Conference Organizer Agent) where the end goal is to create a CFP submission for a developer conference, that meets certain criteria.

1. When the program has finished, go to the Azure AI Foundry portal and click on the **Agents** tab. You should see two agents created: `Software Developer Agent` and `Conference Organizater Agent`. 

1. Select an agent to see the details about the agent, including its instructions, knowledge and actions (there are no for this sample)

1. Select the **Thread** tab to see the conversation between the two agents. You should see a conversation where the two agents are discussing the CFP submission.

## Additional optional task
1. Try adding another agent to the mix, perhaps a CodeOfConduct agent that can also review the CFP submission to ensure it meets the code of conduct requirements. You can also try adding some knowledge to this agent, perhaps a document describing the Code of Conduct (instead of entering a lot of rules in the instructions).

