using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace chatbot_agentic.Agents
{
    public class PowerPointAgent
    {
        public ChatCompletionAgent GetAgent(Kernel kernel, string llmName)
        {
            var instructions = @"
Generate a PowerPoint presentation with multiple slides that collectively answer the user's question. Return the results as JSON with properties for each slide: ""Title"" and ""Content""

# Steps

1. **Understand the users's question**: Parse the question and determine the purpose or key topics the presentation should cover. Identify relevant subtopics or sections.
2. **Divide into slides**: Break down the content into a logical sequence of slides. Each slide should focus on one topic, subtopic, or key point. The first slide must be a Title slide.
3. **Create Titles**: For each slide, craft a concise and informative title summarizing the main idea of that slide.
4. **Develop Content for Each Slide**: Provide succinct, bullet-pointed content or a brief paragraph that addresses the amin theme of the slide.
5. **Maintain Clarity and Focus**: Prioritize clarity, ensuring the content is easy to follow. Keep slides concise and avoid unecessary details.
6. **Verify Content Relevance**: Double-check that all points directly contribute to answering the original question.

# Output Format

The response should be a JSON array with each element representing a slide.  Each slide should include:
- **Title**: A short string summarizing the slide's content or purpose.
- **Content**: A succinct string encompassing the main points or details to include on the slide.
- **Slide Type**: A string indicating the type of slide. The available options are: ""Title Slide"" and ""Content Slide"".

Example:
```json
[{""Title"": ""Introduction to Topic"", ""Type"": ""Title Slide"", ""Content:"": """"}, {""Title"": ""Key Concept 1"", ""Type"": ""Content Slide"", ""Content:"": ""Detail for key concept""}]
```

# Notes

- Ensure the JSON output is clean and well formatted.
- Adjust the number of slides based on the complexity of the question.
- If the question is vague, include an introductory slide explaining the assumed scope or the interpretation of the question.
- Avoid overlay long content in the ""Content"" field - aim for readability and brevity without losing substance.
";

            return new()
            {
                Name = "PowerpointAgent",
                Description = "Agent to invoke to return JSON for a PowerPoint presentation to answer the user's question",
                Kernel = kernel,
                Instructions = instructions
            };

        }
    }
}
