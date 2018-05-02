# Excel Data To FIWARE Orion Context Broker Transfer Utility
A .NET console client library to read data from excel and push to FIWARE Orion Context Broker


# FIWARE.Orion.Client
A .NET portable client library for the FIWARE Orion Context Broker

# Introduction

This is a .NET portable library for the FIWARE Orion Context Broker. Orion exposes interfaces to receive, query and subscribe for context updates. 

This client works with the Globale Instance of Orion as well as with any other deployed instance. 

For more information about Orion, please consult the following documents:

GitHub Repository: https://github.com/telefonicaid/fiware-orion

FIWARE Catalogue: http://catalogue.fiware.org/enablers/publishsubscribe-context-broker-orion-context-broker

FIWARE User Guide: https://forge.fiware.org/plugins/mediawiki/wiki/fiware/index.php/Publish/Subscribe_Broker_-_Orion_Context_Broker_-_User_and_Programmers_Guide

# Installation

Get the Nuget package here: https://www.nuget.org/packages/FIWARE.Orion.Client

Id: FIWARE.Orion.Client

# Usage

## 1. Configure the client

To connect to the Global Instance of Orion, create a new configuration by providing your access token from the FIWARE Lab Account.

Read this tutorial to learn how to get a token: https://forge.fiware.org/plugins/mediawiki/wiki/fiware/index.php/Publish/Subscribe_Broker_-_Orion_Context_Broker_-_Quick_Start_for_Programmers

        OrionClient.OrionConfig config = new OrionClient.OrionConfig()
        {
          Token = "YOUR_ORION_GLOBAL_INSTANCE_TOKEN"
        };
        
        OrionClient client = new OrionClient(config);

To connect to your own instance, configure the base url and your access token, if required.

        OrionClient.OrionConfig config = new OrionClient.OrionConfig()
        {
          Token = "YOUR_ORION_TOKEN",
          BaseUrl = "http://x.x.x.x:1026"
        };
        
        OrionClient client = new OrionClient(config);

## 2. Send a Context Update

        ContextUpdate create = new ContextUpdate()
        {
          UpdateAction = UpdateActionTypes.APPEND,
          ContextElements = new List<ContextElement>(){
            new ContextElement(){
              Type = "Room",
              IsPattern = false,
              Id = "Room1",
              Attributes = new List<Orion.Client.Models.ContextAttribute>(){
                new Orion.Client.Models.ContextAttribute(
                  Name = "temperature",
                  Type = "string",
                  Value = "23",
                }
              }
            },
          }
        };

        ContextResponses createResponses = await client.UpdateContextAsync(create);

## 3. Query the Context Broker

        ContextQuery query = new ContextQuery()
            {
                Entities = new List<ContextQueryEntity>(){
                    new ContextQueryEntity(){
                        Type = "Room",
                        IsPattern = true,
                        Id = "Room.*",
                    },
                },
                Attributes = new List<string>()
                {
                    "temperature",
                }
            };

        ContextResponses queryResponses = await client.QueryAsync(query);

## 4. Create a subscription

            ContextSubscription subscription = new ContextSubscription()
            {
                Entities = new List<ContextQueryEntity>()
                {
                    new ContextQueryEntity(){
                        Type = "Room",
                        IsPattern = true,
                        Id = "Room.*"
                    },
                },
                Attributes = new List<string>() { 
                    "temperature"
                },
                Reference = "YOUR_CALLBACK_URI",
                Duration = SubscriptionDurations.OneMonth,
                NotifyConditions = new List<NotifyCondition>()
                {
                    new NotifyCondition(){
                        Type = NotifyConditionTypes.ONCHANGE,
                        ConditionValues = new List<string>(){ "temperature"}
                    }
                },
                Throttling = SubscriptionThrottlingTypes.PT5S
            };

            ContextSubscriptionResponse subscriptionResponse = await client.SubscribeAsync(subscription);
