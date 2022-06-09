// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.15.2

using EchoBot1.StringMatching;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot1.Bots
{
    public class EchoBot : ActivityHandler
    {
        private readonly BotState _userState;
        private readonly BotState _conversationState;
        public EchoBot(ConversationState conversationState, UserState userState)
        {
            _conversationState = conversationState;
            _userState = userState;
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var conversationStateAccessors = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationData = await conversationStateAccessors.GetAsync(turnContext, () => new ConversationData());

            var userStateAccessors = _userState.CreateProperty<UserProfile>(nameof(UserProfile));
            var userProfile = await userStateAccessors.GetAsync(turnContext, () => new UserProfile());

            if (string.IsNullOrEmpty(userProfile.Name))
            {
                // First time around this is set to false, so we will prompt user for name.
                if (conversationData.PromptedUserForName)
                {
                    // Set the name to what the user provided.
                    userProfile.Name = turnContext.Activity.Text?.Trim();

                    // Acknowledge that we got their name.
                    //await turnContext.SendActivityAsync($"Obrigado {userProfile.Name}. Usaremos seus dados de forma responsável.");

                    await turnContext.SendActivitiesAsync(
                           new Activity[] {
                            new Activity { Type = ActivityTypes.Typing },
                            new Activity { Type = "delay", Value= 3000 },
                            MessageFactory.Text($"Obrigado {userProfile.Name}. Usaremos seus dados de forma responsável.",$"Obrigado {userProfile.Name}. Usaremos seus dados de forma responsável."),
                            new Activity { Type = ActivityTypes.Typing },
                            new Activity { Type = "delay", Value= 3000 },
                            MessageFactory.Text($"Agora digite uma palavra para compararmos:"),
                           },
                           cancellationToken);

                    // Reset the flag to allow the bot to go through the cycle again.
                    conversationData.PromptedUserForName = false;

                    await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
                    await _userState.SaveChangesAsync(turnContext, false, cancellationToken); 
                }
                else
                {
                    // Prompt the user for their name.
                    await turnContext.SendActivityAsync($"Qual é o seu nome?");

                    // Set the flag to true, so we don't prompt in the next turn.
                    conversationData.PromptedUserForName = true;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(conversationData.FirstWord))
                {
                    conversationData.FirstWord = turnContext.Activity.Text.ToLower();

                    await turnContext.SendActivitiesAsync(
                           new Activity[] {
                            new Activity { Type = ActivityTypes.Typing },
                            new Activity { Type = "delay", Value= 3000 },
                            MessageFactory.Text($"Beleza!"),
                            new Activity { Type = ActivityTypes.Typing },
                            new Activity { Type = "delay", Value= 3000 },
                            MessageFactory.Text($"Agora digite a segunda palavra:"),
                           },
                           cancellationToken);

                    await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
                }
                else
                {
                    conversationData.SecondoWord = turnContext.Activity.Text.ToLower();

                    double resultado = ComputeLevenshteinDistance.CalculateSimilarity(conversationData.FirstWord, conversationData.SecondoWord);

                    var replyText = $"{conversationData.FirstWord} é {resultado * 100.0}% parecida com a palavra: {conversationData.SecondoWord}.";

                    await turnContext.SendActivitiesAsync(
                           new Activity[] {
                            new Activity { Type = ActivityTypes.Typing },
                            new Activity { Type = "delay", Value= 3000 },
                            MessageFactory.Text($"Aguarde {userProfile.Name}! Estamos comparando."),
                            new Activity { Type = ActivityTypes.Typing },
                            new Activity { Type = "delay", Value= 5000 },
                            MessageFactory.Text(replyText,replyText),
                           },
                           cancellationToken);

                    await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
                    await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
                }
                
            }
            
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivitiesAsync(
                           new Activity[] {
                            new Activity { Type = ActivityTypes.Typing },
                            new Activity { Type = "delay", Value= 3000 },
                            MessageFactory.Text($"Olá! Bem vindo ao bot comparador de palavras"),
                            new Activity { Type = ActivityTypes.Typing },
                            new Activity { Type = "delay", Value= 3000 },
                            MessageFactory.Text($"Digite qualque coisa: "),
                           },
                           cancellationToken);
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occurred during the turn.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }
    }
}
